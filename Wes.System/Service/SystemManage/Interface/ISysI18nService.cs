using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysI18nService
    {
        public List<SysI18nEntity> GetList(ParamData<I18nParam> param, out int total);

        public SysI18nEntity GetById(long id);

        public bool Save(SysI18nEntity model);

        public bool Delete(List<long> ids);

        /// <summary>按翻译键软删除全部语言条目（如删除菜单时清理 sys.menu.{id} 词条）</summary>
        public bool DeleteByKey(string i18nKey);

        public string GetTranslation(string key, string lang);

        public Dictionary<string, string> GetTranslationsByPrefix(string prefix, string lang);

        public Dictionary<string, string> GetAllFrontendTranslations(string lang);

        public SysI18nEntity GetByKeyAndLang(string i18nKey, string lang);
    }
}
