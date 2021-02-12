using GoldenSIM.Global;
using GoldenSIM.Helpers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Security;

namespace GoldenSIM.ApplicationAccouns
{
    public static class ApplicationAccountManager
    {    /// <summary>
         /// Получить менеджеров
         /// </summary>
         /// <returns></returns>
        public static IEnumerable<ApplicationAccount> GetManagers()
        {
            //выбираем только менеджеров
            var Groups = new int[] { (int)ApplicationAccountRoles.Manager };
            return GetAccounts(
                string.Empty,//поиск по тексту
                null,//массив менеджеров
                null,
                Groups, //массив груп
                null,
                null,
                null,
                string.Empty
                );
        }
        /// <summary>
        /// Получить аккаунт
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static ApplicationAccount GetAccount(int Id)
        {
            return GetAccounts(string.Empty, null, null, null, null, null, Id, string.Empty).SingleOrDefault(x => x.Id.Equals(Id));
        }
        /// <summary>
        /// Получить аккаунт
        /// </summary>
        /// <param name="Login"></param>
        /// <returns></returns>
        public static ApplicationAccount GetAccount(string Login)
        {
            if (string.IsNullOrEmpty(Login)) throw new ArgumentNullException();
            return GetAccounts(string.Empty, null, null, null, null, null, null, Login).SingleOrDefault(x => x.Login.Equals(Login));
        }
        /// <summary>
        /// Получить торговые точки
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApplicationAccount> GetSalesPoints()
        {
            //выбираем только дилеров
            var Groups = new int[] { (int)ApplicationAccountRoles.Diler };
            return GetAccounts(
                string.Empty,//поиск по тексту
                null,//массив менеджеров
                null,
                Groups, //массив груп
                null,
                null,
                null,
                string.Empty
                );
        }
        /// <summary>
        /// Получить торговые точки
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Managers"></param>
        /// <param name="IsApproved"></param>
        /// <param name="Regions"></param>
        /// <param name="Operators"></param>
        /// <returns></returns>
        public static IEnumerable<ApplicationAccount> GetSalesPoints(string Text, int[] Managers, byte? IsApproved, int[] Regions, int[] Operators)
        {
            //выбираем только дилеров
            var Groups = new int[] { (int)ApplicationAccountRoles.Diler };
            return GetAccounts(
                Text,//поиск по тексту
                Managers,//массив менеджеров
                (byte)IsApproved,
                Groups, //массив груп
                Regions, //массив регионов
                Operators,//массив операторов
                null,
                string.Empty
                );
        }
        /// <summary>
        /// Получить пользователей
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Managers"></param>
        /// <param name="IsApproved"></param>
        /// <param name="Groups"></param>
        /// <returns></returns>
        public static IEnumerable<ApplicationAccount> GetUsers(string Text, int[] Managers, byte? IsApproved, int[] Groups)
        {
            //выбираем все группы кроме диллеров
            if (Groups == null) Groups = Enum.GetValues(typeof(ApplicationAccountRoles)).Cast<ApplicationAccountRoles>().Where(x => x != ApplicationAccountRoles.Diler).Select(x => (int)x).ToArray();

            return GetAccounts(
                Text,//поиск по тексту
                Managers,//массив менеджеров
                (byte)IsApproved,
                Groups, //массив груп
                null,
                null,
                null,
                string.Empty
                );
        }
        /// <summary>
        /// Получить аккаунты
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApplicationAccount> GetAccounts()
        {
            return GetAccounts(string.Empty, null, null, null, null, null, null, string.Empty);
        }
        /// <summary>
        /// Получить аккаунты
        /// </summary>
        /// <param name="IsApproved"></param>
        /// <returns></returns>
        public static IEnumerable<ApplicationAccount> GetAccounts(byte? IsApproved)
        {
            return GetAccounts(string.Empty, null, IsApproved, null, null, null, null, string.Empty);
        }
        /// <summary>
        /// Получить аккаунты
        /// </summary>
        /// <param name="IsApproved"></param>
        /// <param name="Groups"></param>
        /// <returns></returns>
        public static IEnumerable<ApplicationAccount> GetAccounts(byte? IsApproved, int[] Groups)
        {
            return GetAccounts(string.Empty, null, IsApproved, Groups, null, null, null, string.Empty);
        }
        /// <summary>
        /// Получить аккаунты
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Managers"></param>
        /// <param name="IsApproved"></param>
        /// <param name="Groups"></param>
        /// <param name="Regions"></param>
        /// <param name="Operators"></param>
        /// <returns></returns>
        private static IEnumerable<ApplicationAccount> GetAccounts(string Text, int[] Managers, byte? IsApproved, int[] Groups, int[] Regions, int[] Operators, int? Id, string Login)
        {
            var AllAccounts = new List<ApplicationAccount>();
            // список агентов
            var Agents = new List<int>();

            using (var connection = new MySqlConnection(Const.DatabaseConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    // запрашиваем аккаунты
                    command.CommandText = $@"SELECT 
	                                        my_aspnet_membership.IsApproved AS IsApproved,
                                            my_aspnet_membership.userId AS Id,
                                            my_aspnet_users.name AS Login,
                                            my_aspnet_roles.Id AS IdRole,
                                            my_aspnet_membership.Password AS Password,
                                            my_aspnet_membership.PasswordQuestion AS PasswordQuestion,
                                            my_aspnet_membership.Email AS Email,
                                            my_aspnet_membership.CreationDate AS CreationDate,
                                            my_aspnet_membership.LastActivityDate AS LastActivityDate,
                                            my_aspnet_membership.LastLoginDate AS LastLoginDate,
                                            my_aspnet_membership.LastPasswordChangedDate AS LastPasswordChangedDate,
                                            my_aspnet_membership.LastLockedOutDate AS LastLockedOutDate,
                                            my_aspnet_membership.IsLockedOut AS IsLockedOut,
                                            CONVERT(profile USING utf8) AS profile
                                        FROM
	                                        my_aspnet_roles
	                                        INNER JOIN my_aspnet_usersinroles ON my_aspnet_usersinroles.roleId = my_aspnet_roles.Id
	                                        INNER JOIN my_aspnet_membership ON my_aspnet_usersinroles.userId = my_aspnet_membership.userId 
	                                        INNER JOIN my_aspnet_users ON my_aspnet_users.id = my_aspnet_membership.userId 
	                                        LEFT JOIN db_users_profiles ON db_users_profiles.userId = my_aspnet_membership.userId
                                        WHERE
                                                my_aspnet_roles.id = 4
                                            OR (            
                                                {(IsApproved != null ? "my_aspnet_membership.IsApproved = @V1" : "my_aspnet_membership.IsApproved in (0,1)")}
                                                {(Groups != null ? " AND my_aspnet_roles.id IN({VGroups})" : string.Empty)}
                                                {(Id != null ? " AND my_aspnet_membership.userId = @V2" : string.Empty)}
                                                {(!string.IsNullOrEmpty(Login) ? " AND my_aspnet_users.name = V3" : string.Empty)}
                                                )";

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("V1", IsApproved);
                    command.AddArrayParameters("VGroups", Groups);
                    command.Parameters.AddWithValue("V2", Id);
                    command.Parameters.AddWithValue("V3", Login);
                    try
                    {
                        if (connection.State == ConnectionState.Closed) connection.Open();
                        using (var dataReader = command.ExecuteReader())
                        {
                            if (dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    var IdRole = dataReader.GetInt32("IdRole");
                                    ApplicationAccount NewAccount;
                                    if ((ApplicationAccountRoles)IdRole == ApplicationAccountRoles.Diler)
                                        NewAccount = new SalesPoint(dataReader["profile"].ToString());
                                    else
                                        NewAccount = new ApplicationUser(dataReader["profile"].ToString());

                                    NewAccount.Id = Convert.ToInt32(dataReader["Id"].ToString());
                                    NewAccount.Login = dataReader["Login"].ToString();
                                    NewAccount.Email = dataReader["Email"].ToString();
                                    NewAccount.Password = dataReader["Password"].ToString();
                                    NewAccount.PasswordQuestion = dataReader["PasswordQuestion"].ToString();
                                    NewAccount.CreationDate = dataReader["CreationDate"].ToString();
                                    NewAccount.LastPasswordChangedDate = dataReader["LastPasswordChangedDate"].ToString();
                                    NewAccount.LastLockedOutDate = dataReader["LastLockedOutDate"].ToString();
                                    NewAccount.LastActivityDate = dataReader["LastActivityDate"].ToString();
                                    NewAccount.LastLoginDate = dataReader["LastLoginDate"].ToString();
                                    NewAccount.IsLockedOut = dataReader.GetByte("IsLockedOut");
                                    NewAccount.IsApproved = dataReader.GetByte("IsApproved");
                                    NewAccount.IdRole = dataReader.GetInt32("IdRole");
                                    AllAccounts.Add(NewAccount);

                                    // формируем список агентов, которые требуется запросить отдельно
                                    if (NewAccount is SalesPoint)
                                        if (((SalesPoint)NewAccount).IdAgent != null)
                                            Agents.Add((int)((SalesPoint)NewAccount).IdAgent);
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        GoldenSIM.Core.Logs.RegisterError(GoldenSIM.Global.Const.DatabaseConnectionString, MethodBase.GetCurrentMethod().DeclaringType, ref exception, new StackTrace(false).GetFrame(0).GetMethod().Name);
                        return null;
                    }
                }

            }
            //убираем дубли
            Agents.Distinct();
            //получаем агентов
            List<Provider> Providers = ProviderManager.GetProviders(Agents).ToList();

            foreach (var Account in AllAccounts)
            {
                //заполняем поле Manager у всех аккаунтов
                if (Account.IdManager != -1)
                    Account.Manager = AllAccounts.SingleOrDefault(x => x.Id.Equals(Account.IdManager))?.FullName;
                //запоняем сведения об агенте для торговых точек
                if (Account is SalesPoint)
                    if (((SalesPoint)Account).IdAgent != null)
                        ((SalesPoint)Account).provider = Providers.SingleOrDefault(x => x.Id.Equals((int)((SalesPoint)Account).IdAgent));
            }

            #region фильтры
            // *********** применяем фильтры*********
            if (IsApproved != null) AllAccounts = AllAccounts.Where(x => x.IsApproved.Equals(IsApproved)).ToList();
            //по группам
            if (Groups?.Count() > 0) AllAccounts = AllAccounts.Where(x => Groups.Any(y => y == x.IdRole)).ToList();
            //по тексту
            if (!string.IsNullOrEmpty(Text)) AllAccounts = AllAccounts
                    .Where(x =>
                (!string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Contains(Text)) ||
                (!string.IsNullOrEmpty(x.Login) && x.Login.ToLower().Contains(Text)) ||
                (!string.IsNullOrEmpty(x.FullName) && x.FullName.ToLower().Contains(Text))
                ).ToList();
            //по менеджерам
            if (Managers?.Count() > 0) AllAccounts = AllAccounts.Where(x => Managers.Any(y => y == x.IdManager)).ToList();
            //по региону для торговых точек
            if (Regions?.Count() > 0) AllAccounts = AllAccounts.Where(x => x is SalesPoint && Regions.Any(y => y == ((SalesPoint)x).RegionId)).ToList();
            //по оператору для торговых точек
            if (Operators?.Count() > 0) AllAccounts = AllAccounts.Where(x => x is SalesPoint && Operators.Any(y => y == ((SalesPoint)x).IdOperator)).ToList();
            //по ID
            if (Id != null) AllAccounts = AllAccounts.Where(x => x.Id.Equals(Id)).ToList();
            //по логину
            if (!string.IsNullOrEmpty(Login)) AllAccounts = AllAccounts.Where(x => x.Login.Equals(Login)).ToList();
            #endregion

            // применяем фильтры
            //AllAccounts = (from x in AllAccounts
            //               where
            //                  (IsApproved != null && x.IsApproved.Equals(IsApproved)) &
            //                  (Groups?.Count() > 0 && Groups.Any(y => y == x.IdRole)) &
            //                  (!string.IsNullOrEmpty(Text) && (x.Name.ToLower().Contains(Text) || x.Login.ToLower().Contains(Text) || x.FullName.ToLower().Contains(Text))) &
            //                  (Managers?.Count() > 0 && (x is SalesPoint && Regions.Any(y => y == ((SalesPoint)x).RegionId))) &
            //                  (Regions?.Count() > 0 && (x is SalesPoint && Regions.Any(y => y == ((SalesPoint)x).RegionId))) &
            //                  (Operators?.Count() > 0 && (x is SalesPoint && Operators.Any(y => y == ((SalesPoint)x).IdOperator)))
            //               select x).ToList();


            return AllAccounts;
        }
        /// <summary>
        /// Сохранить аккаунт
        /// </summary>
        /// <param name="Account"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static bool SaveAccount(ApplicationAccount Account, out string Message)
        {
            if (Account == null) throw new ArgumentNullException();
            if (Account.Id == null)
                return CreateAccount(Account, out Message);
            else
                return UpdateAccount(Account, out Message);
        }

        /// <summary>
        /// Создание нового аккаунта
        /// </summary>
        /// <param name="Account"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        private static bool CreateAccount(ApplicationAccount Account, out string Message)
        {
            if (Account == null) throw new ArgumentNullException();
            if (Account is ApplicationUser && (string.IsNullOrEmpty(Account.Login) || string.IsNullOrEmpty(Account.Password)))
            {
                Message = OperationResult.ArgumentsIsNull.GetDescription();
                return false;
            }
            if (Account is SalesPoint)
            {
                if (string.IsNullOrEmpty(Account.Login))
                {
                    Regex regex = new Regex("GS[0-9]{6}$");
                    var login_max_id = GetAccounts().Where(x => regex.IsMatch(x.Login)).OrderByDescending(x => x.Login).ToArray()[0].Login;
                    Account.Login = $"GS{string.Format("{0:000000}", Convert.ToInt32(login_max_id.OnlyDigital()) + 1)}";
                }
                if (string.IsNullOrEmpty(Account.Password)) Account.Password = Membership.GeneratePassword(7, 1);
                Account.IdRole = (int)ApplicationAccountRoles.Diler;
            }
            try
            {
                MembershipCreateStatus status;
                var NewAccount = Membership.CreateUser(
                    Account.Login,
                    Account.Password,
                    Account.Email,
                    !string.IsNullOrEmpty(Account.PasswordQuestion) ? Account.PasswordQuestion : "Yes",
                    !string.IsNullOrEmpty(Account.PasswordAnswer) ? Account.PasswordAnswer : "No",
                    true,
                    out status);
                if (NewAccount == null || status != MembershipCreateStatus.Success)
                {
                    Message = $"{OperationResult.CreateAccountError.GetDescription()} [{status}]";
                    return false;
                }
                //установка роли
                Roles.AddUserToRole(Account.Login, Account.Role);
                //сохранение профиля
                if (!SaveProfile((int)NewAccount.ProviderUserKey, Account.Profile, out Message)) return false;
                //блокирование аккаунта при необходимости
                if (Account.IsLockedOut.Equals((byte)ApplicationAccountStatus.locked)) NewAccount.LockUser();

                Message = OperationResult.OK.GetDescription();
                return true;
            }
            catch (Exception e)
            {
                GoldenSIM.Core.Logs.RegisterError(Const.DatabaseConnectionString, MethodBase.GetCurrentMethod().DeclaringType, ref e, new StackTrace(false).GetFrame(0).GetMethod().Name);
                Message = OperationResult.InternalServerError.GetDescription();
                return false;
            }
        }
        /// <summary>
        /// Обновление аккаунта
        /// </summary>
        /// <param name="Account"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        private static bool UpdateAccount(ApplicationAccount Account, out string Message)
        {
            if (Account == null) throw new ArgumentNullException();

            var membershipUser = Membership.GetUser(Account.Id);

            membershipUser.Email = Account.Email;
            //сохранение пользователя
            Membership.UpdateUser(membershipUser);

            if (Account is ApplicationUser)
            {
                // изменение роли
                var curentRoles = Roles.GetRolesForUser(membershipUser.UserName);
                if (curentRoles.Length > 0) Roles.RemoveUserFromRoles(membershipUser.UserName, curentRoles);
                Roles.AddUserToRole(Account.Login, ((ApplicationAccountRoles)Account.IdRole).ToString());
            }
            // изменение пароля
            if (!membershipUser.IsLockedOut && !string.IsNullOrEmpty(Account.Password))
            {
                string oldPassword = membershipUser.GetPassword();
                if (!oldPassword.Equals(Account.Password))
                    try
                    {
                        membershipUser.ChangePassword(oldPassword, Account.Password);
                    }
                    catch (ArgumentException)
                    {
                        Message = OperationResult.BadPassword.GetDescription();
                        return false;
                    }
                    catch (Exception e)
                    {
                        GoldenSIM.Core.Logs.RegisterError(Const.DatabaseConnectionString, MethodBase.GetCurrentMethod().DeclaringType, ref e, new StackTrace(false).GetFrame(0).GetMethod().Name);
                        Message = OperationResult.InternalServerError.GetDescription();
                        return false;
                    }
                // изменение контрольного вопроса и ответа
                if (!string.IsNullOrEmpty(Account.PasswordQuestion) && !string.IsNullOrEmpty(Account.PasswordAnswer))
                    membershipUser.ChangePasswordQuestionAndAnswer(membershipUser.GetPassword(), Account.PasswordQuestion, Account.PasswordAnswer);
            }
            //блокирование аккаунта при необходимости
            if (Account.IsLockedOut.Equals((byte)ApplicationAccountStatus.locked)) membershipUser.LockUser();
            // сохранение профиля
            if (!SaveProfile((int)Account.Id, Account.Profile, out Message)) return false;

            Message = OperationResult.OK.GetDescription();
            return true;
        }
        /// <summary>
        /// Сохранение профиля
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Profile"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        private static bool SaveProfile(int Id, ApplicationAccountProfile Profile, out string Message)
        {
            if (Profile == null) throw new ArgumentNullException();
            try
            {
                using (var connection = new MySqlConnection(Const.DatabaseConnectionString))
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO db_users_profiles (userId,profile) VALUES(@V1,@V2) ON DUPLICATE KEY UPDATE profile=@V2";
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("V1", Id);
                        command.Parameters.AddWithValue("V2", Profile.Json);
                        if (command.Connection.State == ConnectionState.Closed) command.Connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception exception)
            {
                GoldenSIM.Core.Logs.RegisterError(Const.DatabaseConnectionString, MethodBase.GetCurrentMethod().DeclaringType, ref exception, new StackTrace(false).GetFrame(0).GetMethod().Name);
                Message = OperationResult.ProfileSaveError.GetDescription();
                return false;
            }
            Message = OperationResult.OK.GetDescription();
            return true;
        }
        /// <summary>
        /// Включение/отключение аккаунта
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static bool DisableAccount(int? Id, out string Message)
        {
            if (Id == null) throw new ArgumentNullException();
            var Account = Membership.GetUser((int)Id);
            if (Account == null)
            {
                Message = OperationResult.AccountNotFound.GetDescription();
                return false;
            }
            Account.IsApproved = !Account.IsApproved;
            Membership.UpdateUser(Account);
            Message = OperationResult.OK.GetDescription();
            return true;
        }
        /// <summary>
        /// проверка разрешения доступа пользователя на сайт по IP адресу
        /// </summary>
        /// <param name="User"></param>
        /// <param name="IP"></param>
        /// <returns></returns>
        public static bool CheckingAccessByIP(this MembershipUser User, string IP)
        {
            if (string.IsNullOrEmpty(IP)) IP = "127.0.0.1";
            if (IP.Equals("::1")) IP = "127.0.0.1";
            var Account = GetAccount((int)User.ProviderUserKey);
            var IpAddreses = Account.IPaddreses;
            // если в профиле не указаны адреса доступа - доступ разрешен
            if (IpAddreses.Count == 0) return true;

            foreach (var item in IpAddreses)
            {
                // диапазон
                if (item.IndexOf('-') != -1)
                {
                    var range = item.Split('-');
                    if (range.Length != 2) continue;
                    // адрес попадает в диапазон
                    if (IPtoDouble(IP) >= IPtoDouble(range[0]) && IPtoDouble(IP) <= IPtoDouble(range[1])) return true;
                }
                // дипазон, указанный по маске
                else if (item.IndexOf('/') != -1)
                {
                    var address = item.Split('/');
                    if (address.Length != 2) continue;
                    int mask;
                    if (!int.TryParse(address[1], out mask) || mask < 2 || mask > 32) continue;
                    var beginip = IPtoDouble(address[0]);
                    var endip = IPtoDouble(address[0] + Masks[mask]);
                    // адрес попадает в диапазон
                    if (IPtoDouble(IP) >= beginip && IPtoDouble(IP) <= endip) return true;
                }
                // одиночный IP
                else
                    if (IPtoDouble(IP) == IPtoDouble(item)) return true;

            }

            return false;
        }
        //соответствие количества сетевых адресов маске, указанной через слеш
        private static readonly Dictionary<int, int> Masks = new Dictionary<int, int>()
    {
        { 32,0 }, {31,1 }, {30,3 }, {29,7 }, {28,15 }, {27,31 }, {26,63 },
        { 25,127 }, {24,255 }, {23,511 }, {22,1023 }, {21,2047 }, {20,4095 },
        { 19,8191 }, {18,16383 }, {17,32767 }, {16,65535 }, {15,131071 },
        { 14,262143 }, {13,524287 }, {12,1048575 }, {11,2097151 },
        { 10,4194303 }, {9,8388607 }, {8,16777215 }, {7,33554431 },
        { 6,67108863 }, {5,134217727 }, {4,268435455 }, {3,536870911 },
        { 2,1073741823 }
        };
        // перевод строчного представления IP-адреса в число
        private static double? IPtoDouble(string Ip)
        {
            var OctetsStr = Ip.Split('.');
            if (OctetsStr.Length != 4) return null;

            var Octets = new int[4];
            for (var i = 0; i < 4; i++)
            {
                int temp;
                if (int.TryParse(OctetsStr[i], out temp) && temp >= 0 && temp <= 255)
                    Octets[i] = temp;
                else
                    return null;
            }
            var result = Octets[0] * Math.Pow(256, 3) + Octets[1] * Math.Pow(256, 2) + Octets[2] * Math.Pow(256, 1) + Octets[3] * Math.Pow(256, 0);
            return result;
        }

        /// <summary>
        /// Блокировать аккаунт
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool LockUser(int? Id, out string message)
        {
            var user = Membership.GetUser(Id);
            if (user.LockUser())
            {
                message = OperationResult.AccountLock.GetDescription();
                return true;
            }
            else
            {
                message = OperationResult.AccountLockFail.GetDescription();
                return false;
            }
        }
        /// <summary>
        /// Разблокировать
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool UnlockUser(int? Id, out string message)
        {
            var user = Membership.GetUser(Id);
            if (user.UnlockUser())
            {
                message = OperationResult.AccountUnLock.GetDescription();
                return true;
            }
            else
            {
                message = OperationResult.AccountUnLockFail.GetDescription();
                return false;
            }
        }

    }
}