using UnityEngine;

namespace Storage
{
    public static class PlayerPrefsController
    {
        #region SETTER

        public static void SetLevelIndex(int index) => PlayerPrefs.SetInt("level-index", index);
        public static void SetLevelNumber(int number) => PlayerPrefs.SetInt("level-number", number);
        public static void SetCurrency(int currency) => PlayerPrefs.SetInt("currency", currency);

        #endregion

        #region GETTER

        public static int GetLevelIndex() => PlayerPrefs.GetInt("level-index");
        public static int GetLevelNumber() => PlayerPrefs.GetInt("level-number");
        public static int GetTotalCurrency() => PlayerPrefs.GetInt("currency");

        #endregion
    }
}