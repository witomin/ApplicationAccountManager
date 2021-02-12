using GoldenSIM.Entity.Enum;
using GoldenSIM.Repository.Outlets;
using System.Collections.Generic;

namespace GoldenSIM.ApplicationAccouns
{
    /// <summary>
    /// Профиль торговой точки
    /// </summary>
    public class SalesPointProfile: ApplicationAccountProfile
    {
        /// <summary>
        /// Геолокация
        /// </summary>
        public GPS GEO { get; set; }
        /// <summary>
        /// Финансовые свойства
        /// </summary>
        public FinancialSettings FinancialSettings { get; set; }
        /// <summary>
        /// ID агента
        /// </summary>
        public uint? IdAgent { get; set; }
        /// <summary>
        /// Код региона
        /// </summary>
        public RegionCode RegionIdInternal { get; set; }

        public SalesPointProfile()
        {
            GEO = new GPS();
            Contact = new ApplicationAccountContact();
            FinancialSettings = new FinancialSettings();
            IPaddreses = new List<string>();
        }

    }
}