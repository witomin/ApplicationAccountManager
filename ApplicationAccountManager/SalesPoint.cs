using GoldenSIM.Entity.Enum;
using GoldenSIM.Helpers;
using GoldenSIM.Repository.Outlets;
using Newtonsoft.Json;
using System;
using System.Web.Security;

namespace GoldenSIM.ApplicationAccouns
{
    /// <summary>
    /// Класс торговой точки
    /// </summary>
    public class SalesPoint : ApplicationAccount
    {
        public override dynamic Profile
        {
            get
            {
                var result = new SalesPointProfile
                {
                    Name = Name,
                    Code = Code,
                    Address = Address,
                    GEO = new GPS
                    {
                        Latitude = Latitude,
                        Longitude = Longitude
                    },
                    Contact = new ApplicationAccountContact
                    {
                        FullName = FullName,
                        Phone = Phone,
                        Email = Email
                    },
                    FinancialSettings = new FinancialSettings
                    {
                        BonusPercentage = BonusPercentage,
                        MaximumPayout = MaximumPayout,
                        PercentageOfDeductions = PercentageOfDeductions
                    },
                    IdAgent = IdAgent,
                    IdManager = IdManager,
                    IPaddreses = IPaddreses
                };
                return result;
            }
        }
        public Provider provider
        {
            set
            {
                ParseProvider(value);
            }
        }
        public override string ButtonEdit
        {
            get
            {
                if (IsApproved == 0) return string.Empty;

                return $@"<div style = ""cursor:pointer;  float: left; margin: 5px;""class='ui-state-default ui-corner-all ui-icon ui-icon-pencil dialog-edit' title = 'Изменить' onclick='return updateSalesPoint($(this).parent(`td`).parent(`tr`));'></div>";

            }
        }

        public override string ButtonDelete
        {
            get
            {
                if (IsApproved == 1)
                    return string.Concat("<div style='cursor:pointer; float: left; margin: 5px;' class='ui-state-default ui-corner-all ui-icon ui-icon-close' title = 'Удалить' onclick='if (confirm(\"Вы действительно хотите удалить торговую точку ", Name, "?\")){deleteSalesPoint(", Id, ");}'></div>");
                else
                    return string.Concat("<div style='cursor:pointer; float: left; margin: 5px;' class='ui-state-default ui-corner-all ui-icon ui-icon-arrowrefresh-1-n' title = 'Восстановить' onclick='if (confirm(\"Вы действительно хотите восстановить торговую точку ", Name, "?\")){UnDeleteSalesPoint(", Id, ");}'></div>");
            }
        }
        #region Геолокация
        /// <summary>
        /// Широта
        /// </summary>
        public float? Latitude { get; set; }
        /// <summary>
        /// Долгота
        /// </summary>
        public float? Longitude { get; set; }
        #endregion
        #region Финансовые свойства
        /// <summary>
        /// Процент отчисления
        /// </summary>
        public int? PercentageOfDeductions { get; set; }
        /// <summary>
        /// Максимальная выплата
        /// </summary>
        public int? MaximumPayout { get; set; }
        /// <summary>
        /// Бонус %
        /// </summary>
        public int? BonusPercentage { get; set; }
        #endregion
        #region Агент
        /// <summary>
        /// ID агента
        /// </summary>
        public uint? IdAgent { get; set; }
        /// <summary>
        /// Id оператора
        /// </summary>
        public int IdOperator { get; set; }
        /// <summary>
        /// Наименование оператора
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// Код региона
        /// </summary>
        public int RegionId { get; set; }
        /// <summary>
        /// Регион
        /// </summary>
        public string RegionName
        {
            get
            {
                return ((RegionCode)Convert.ToInt32(RegionId)).GetDescription(LangId.RUS);
            }
        }
        /// <summary>
        /// Код агента
        /// </summary>
        public string Article { get; set; }
        /// <summary>
        /// Внутреннее наименование агента
        /// </summary>
        public string AgentNameInternal { get; set; }
        /// <summary>
        /// Наименование агента
        /// </summary>
        public string AgentName { get; set; }
        #endregion
        public SalesPoint()
        {
        }
        public SalesPoint(string Profile)
        {
            ParseProfile(Profile);
        }
        public SalesPoint(SalesPointProfile Profile, ApplicationUser User, Provider Provider)
        {
            ParseProfile(Profile);
            ParseProvider(Provider);
            ParseUser(User);
        }

        private void ParseProfile(string Profile)
        {
            var newProfile = JsonConvert.DeserializeObject<SalesPointProfile>(Profile);
            ParseProfile(newProfile);
        }
        private void ParseUser(ApplicationUser user)
        {
            Id = Convert.ToInt32(user.Id);
            Login = user.Login;
            Password = user.Password;
            IsLockedOut = user.IsLockedOut;
            CreationDate = user.CreationDate;
            Email = user.Email;
            IsApproved = user.IsApproved;
        }

        public SalesPoint(SalesPointProfile Profile, MembershipUser User, Provider Provider)
        {
            ParseProfile(Profile);
            ParseProvider(Provider);
            ParseUser(User);

        }
        /// <summary>
        /// Парсинг пользователя
        /// </summary>
        /// <param name="user"></param>
        private void ParseUser(MembershipUser user)
        {
            Id = Convert.ToInt32(user.ProviderUserKey);
            Login = user.UserName;
            Password = user.IsLockedOut ? string.Empty : user.GetPassword();
            IsLockedOut = Convert.ToByte(user.IsLockedOut);
            CreationDate = user.CreationDate.ToString();
            Email = user.Email;
            IsApproved = Convert.ToByte(user.IsApproved);
        }
        /// <summary>
        /// Парсинг агента
        /// </summary>
        /// <param name="provider"></param>
        private void ParseProvider(Provider provider)
        {
            if (provider == null) return;
            IdOperator = provider.Id_operator;
            Operator = provider.Operator;
            RegionId = provider.RegionId;
            Article = provider.Article;
            AgentName = provider.Name;
            AgentNameInternal = provider.AgentName;
        }

        /// <summary>
        /// Парсинг профиля
        /// </summary>
        /// <param name="profile"></param>

        internal override void ParseProfile(dynamic Profile)
        {
            if (Profile is SalesPointProfile)
            {
                Name = Profile.Name;
                Code = Profile.Code;
                Address = Profile.Address;
                IdManager = Profile.IdManager;
                FullName = Profile.Contact.FullName;
                Phone = Profile.Contact.Phone;
                IPaddreses = Profile.IPaddreses;

                Latitude = Profile.GEO.Latitude;
                Longitude = Profile.GEO.Longitude;


                PercentageOfDeductions = Profile.FinancialSettings.PercentageOfDeductions;
                MaximumPayout = Profile.FinancialSettings.MaximumPayout;
                BonusPercentage = Profile.FinancialSettings.BonusPercentage;

                IdAgent = Profile.IdAgent;
            }

        }
    }

}