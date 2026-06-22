using GFramework.Utils;
using System;

namespace GFramework.GameData
{
    public class UserDataBase : StorageDataBase
    {
        public static Action<string> OnDataChanged;

        public UserDataBase()
        {
           
        }
    }
}
