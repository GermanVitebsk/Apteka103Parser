using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;

namespace Apteka103Parser
{
    public class Medicine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
    }

    /*public class MedicineDescription
    {
        public int ID { get; set; }
        public int MedicineID { get; set; }
        public string Composition { get; set; }       // Состав
        public string Description { get; set; }       // Описание
        public string Indications { get; set; }       // Показания
        public string Contraindications { get; set; } // Противопоказания
        public string Dosing { get; set; }            // Способ применения и дозы
        public string Features { get; set; }          // Особенности применения
        public string Form { get; set; }              // Форма выпуска
        public string Storage { get; set; }           // Условия хранения
        public string Expiration { get; set; }        // Срок годности
        public string Conditions { get; set; }        // Условия отпуска из аптек
    }*/
}
