using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Windows;

namespace Apteka103Parser
{
    using Alphabet = List<Letter>;

    internal class MedicineParser
    {
        public static MedicineParserResults Parse()
        {
            var htmlWeb = new HtmlWeb
            {
                OverrideEncoding = Encoding.GetEncoding(1251)
            };
            var document = htmlWeb.Load("http://apteka.103.by/instruktsii/");

            // <div class="ABC"><noindex>...</noindex></div>
            var noindexNodes = document.DocumentNode
                .Descendants("div")
                .Where(div =>
                    div.Attributes.Contains("class")
                    &&
                    div.Attributes["class"].Value.Contains("ABC")
                )
                .First().ChildNodes.Where(cn => cn.Name == "noindex");

            var alphabet = GetAlphabet(noindexNodes);
            return AddNewMedicines(alphabet);
        }

        private static Alphabet GetAlphabet(IEnumerable<HtmlNode> nodes)
        {
            var alphabet = new Alphabet();
            foreach (var node in nodes)
            {
                var child = node.ChildNodes.First();
                alphabet.Add(new Letter(
                    child.InnerText,
                    "http://apteka.103.by" + child.Attributes["href"].Value
                ));
            }
            return alphabet;
        }

        private static MedicineParserResults AddNewMedicines(Alphabet alphabet)
        {
            var rowsAdded = 0;
            var htmlWeb = new HtmlWeb
            {
                OverrideEncoding = Encoding.GetEncoding(1251)
            };
            foreach (var l in alphabet)
            {
                // <ul class="list">
                var document = htmlWeb.Load(l.Url);
                var listNode = document.DocumentNode
                    .Descendants("ul")
                    .Where(n =>
                        n.Attributes.Contains("class")
                        &&
                        n.Attributes["class"].Value.Equals("list")
                    );

                // Получение всех препаратов
                var meds = new List<Medicine>();
                foreach (var ul in listNode)
                foreach (var li in ul.Descendants("li"))
                {
                    var node = li.Descendants("a").First();
                    var med = new Medicine
                    {
                        Name = node.InnerText,
                        Url = node.Attributes["href"].Value == "/-instruktsiya/" 
                            ? null
                            : ("http://apteka.103.by" + node.Attributes["href"].Value)
                    };
                    meds.Add(med);
                }

                using (var context = new MedicineContext())
                {
                    foreach (var med in meds)
                    {
                        if (context.Medicines.Count(m => m.Name == med.Name) > 0)
                            continue;

                        context.Medicines.Add(med);
                        rowsAdded++;
                    }
                    context.SaveChanges();
                }
            }

            var descAdded = ParseDescription();
            return new MedicineParserResults(rowsAdded, descAdded);
        }
        
        public static int ParseDescription()
        {
            var descAdded = 0;
            var htmlWeb = new HtmlWeb
            {
                OverrideEncoding = Encoding.GetEncoding(1251)
            };
            using (var context = new MedicineContext())
            {
                var emptyDescTable = context.Medicines
                    .Where(m =>
                        m.Description == null
                        &&
                        m.Url != null
                    );
                Parallel.ForEach(emptyDescTable,
                    med =>
                    {
                        try
                        {
                            var document = htmlWeb.Load(med.Url);

                            // <div class="medicament">
                            var node = document.DocumentNode
                                .Descendants("div")
                                .Where(div =>
                                    div.Attributes.Contains("class")
                                    &&
                                    div.Attributes["class"].Value.Equals("medicament")
                                )?.First();

                            if (node == null)
                                return;

                            med.Description = node.InnerHtml;
                            descAdded++;
                        }
                        catch (System.Net.WebException e)
                        {
                            MessageBox.Show(
                                med.Url + "\n" + e.Message,
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                            return;
                        }
                    });
                context.SaveChanges();
            }
            return descAdded;
        }
    }

    internal class MedicineParserResults
    {
        public MedicineParserResults() 
        {
        }

        public MedicineParserResults(int newRowsCount, int newDescriptionCount)
        {
            NewRowsCount = newRowsCount;
            NewDescriptionCount = newDescriptionCount;
        }

        public int NewRowsCount { get; set; } = 0;
        public int NewDescriptionCount { get; set; } = 0;
    }

    internal class Letter
    {
        public Letter(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public string Name { get; }
        public string Url { get; }
    }
}