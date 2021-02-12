using GoldenSIM.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Security;

namespace GoldenSIM.ApplicationAccouns
{
    /// <summary>
    /// Абстрактный класс пользователя приложения
    /// </summary>
    public abstract class ApplicationAccount
    {
        /// <summary>
        /// ID пользователя
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// Признак, показывающий, можно ли проверить подлинность пользователя
        /// </summary>
        public byte IsApproved { get; set; }
        /// <summary>
        /// Признак, указывающи является ли пользователь заблокированным
        /// </summary>
        public byte IsLockedOut { get; set; }
        /// <summary>
        /// Расшифровка состояния блокировки
        /// </summary>
        public string IsLockedOutDescription
        {
            get
            {
                return ((ApplicationAccountStatus)IsLockedOut).GetDescription();
            }
        }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// ID роли пользователя
        /// </summary>
        public int? IdRole { get; set; }
        /// <summary>
        /// Название роли пользователя
        /// </summary>
        public string Role
        {
            get
            {
                return IdRole != (int?)null ? ((ApplicationAccountRoles)IdRole).ToString() : string.Empty;
            }
        }
        /// <summary>
        /// Описание роли пользователя
        /// </summary>
        public string RoleName
        {
            get
            {
                return IdRole != (int?)null ? ((ApplicationAccountRoles)IdRole).GetDescription() : string.Empty;
            }
        }
        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        /// <summary>
        /// Дата создания пользователя
        /// </summary>
        public string CreationDate { get; set; }
        /// <summary>
        /// Дата последней активности пользователя
        /// </summary>
        public string LastActivityDate { get; set; }
        /// <summary>
        /// Дата последнего входа
        /// </summary>
        public string LastLoginDate { get; set; }
        /// <summary>
        /// Дата последней смены пароля
        /// </summary>
        public string LastPasswordChangedDate { get; set; }
        /// <summary>
        /// Дата блокировки
        /// </summary>
        public string LastLockedOutDate { get; set; }
        #region Контакт
        /// <summary>
        /// ФИО
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Контактный телефон
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// E-mail
        /// </summary>
        public string Email { get; set; }
        #endregion
        #region Организация
        /// <summary>
        /// Наименование организации
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Код организации
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Фактический адрес
        /// </summary>
        public string Address { get; set; }
        #endregion
        #region Менеджер
        /// <summary>
        /// ID Менеджера
        /// </summary>
        public int IdManager { get; set; }
        /// <summary>
        /// ФИО менеджера
        /// </summary>
        public string Manager { get; set; }
        #endregion
        /// <summary>
        /// Список IP-адресов, с которых разрешен вход в приложение
        /// </summary>
        public List<string> IPaddreses { get; set; }

        /// <summary>
        /// Кнопка "Изменить" для web-форм
        /// </summary>
        public abstract string ButtonEdit { get; }
        /// <summary>
        /// Кнопка "Удалить" для web-форм
        /// </summary>
        public abstract string ButtonDelete { get; }
        /// <summary>
        /// Парсинг профиля
        /// </summary>
        /// <param name="profile"></param>
        internal abstract void ParseProfile(dynamic profile);
        /// <summary>
        /// Профиль пользователя
        /// </summary>
        [JsonIgnore]
        public abstract dynamic Profile { get; }
        /// <summary>
        /// Заблокировать аккаунт
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Lock(out string message)
        {
            var user = Membership.GetUser(Id);
            if (user.LockUser())
            {
                message = "Пользователь успешно заблокирован";
                return true;
            }
            else
            {
                message = "Не удалось заблокировать пользователя.";
                return false;
            }
        }
        /// <summary>
        /// Разблокировать аккаунт
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Unlock(out string message)
        {
            var user = Membership.GetUser(Id);
            if (user.UnlockUser())
            {
                message = "Пользователь успешно разблокирован";
                return true;
            }
            else
            {
                message = "Не удалось разблокировать пользователя.";
                return false;
            }
        }
        public ApplicationAccount()
        {
        }
    }
    /// <summary>
    /// Класс контакта
    /// </summary>
    public class ApplicationAccountContact
    {
        private string fullName;
        private string phone;
        private string email;

        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(fullName))
                    return string.Empty;
                return fullName;
            }
            set
            {
                fullName = value;
            }
        }
        public string Phone
        {
            get
            {
                if (string.IsNullOrEmpty(phone))
                    return string.Empty;
                return phone;
            }
            set
            {
                phone = value;
            }
        }
        public string Email
        {
            get
            {
                if (string.IsNullOrEmpty(email))
                    return string.Empty;
                return email;
            }
            set
            {
                email = value;
            }
        }

    }
    public enum ApplicationAccountStatus
    {
        /// <summary>
        /// Активен
        /// </summary>
        [Description("Активен")]
        active = 0,
        /// <summary>
        /// Блокирован
        /// </summary>
        [Description("Блокирован")]
        locked = 1
    }

    public enum ApplicationAccountRoles
    {
        /// <summary>
        /// Администраторы
        /// </summary>
        [Description("Администраторы")]
        Admin = 1,
        /// <summary>
        /// Дилеры
        /// </summary>
        [Description("Дилеры")]
        Diler = 2,
        /// <summary>
        /// Операторы
        /// </summary>
        [Description("Операторы")]
        Operator = 3,
        /// <summary>
        /// Менеджеры
        /// </summary>
        [Description("Менеджеры")]
        Manager = 4,
        /// <summary>
        /// Заполнители инфокарт
        /// </summary>
        [Description("Заполнители инфокарт")]
        Infowriter = 5,
        /// <summary>
        /// Турагентства
        /// </summary>
        [Description("Турагентства")]
        Tourist = 6,
        /// <summary>
        /// Наполнители контента
        /// </summary>
        [Description("Наполнители контента")]
        Content = 7,
        /// <summary>
        /// Кладовщики
        /// </summary>
        [Description("Кладовщики")]
        Sklad = 8,
        /// <summary>
        /// Продавцы
        /// </summary>
        [Description("Продавцы")]
        Seller = 9,
        /// <summary>
        /// Активаторы
        /// </summary>
        [Description("Активаторы")]
        Activator = 10,
        /// <summary>
        /// Терминалы
        /// </summary>
        [Description("Терминалы")]
        Terminal = 11
    }
}