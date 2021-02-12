using Newtonsoft.Json;
using System.Collections.Generic;

namespace GoldenSIM.ApplicationAccouns
{
    /// <summary>
    /// Абстрактный класс аккаунта
    /// </summary>
    public abstract class ApplicationAccountProfile
    {
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
        /// <summary>
        /// ID Менеджера
        /// </summary>
        public int IdManager { get; set; }
        /// <summary>
        /// список IP-адресов, с которых разрешен доступ
        /// </summary>
        public List<string> IPaddreses { get; set; }
        /// <summary>
        /// Контакс
        /// </summary>
        public ApplicationAccountContact Contact { get; set; }
        /// <summary>
        /// Профиль в формате json
        /// </summary>
        [JsonIgnore]
        public string Json
        {
            get
            {
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented };
                return JsonConvert.SerializeObject(this, Formatting.Indented, jsonSerializerSettings);
            }
        }
    }
}