using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

namespace SS
{
    public static class GameManager
    {
        #region Private
        //private static GameManagerRunnerBase _RunnerInstance;

        private static bool _Initialized = false;

        private static StringFlag bypassInputFlag = new StringFlag();

        public static bool IsControlLocked => !controlLock.IsEmpty;

        private static StringFlag controlLock = new StringFlag();
        private static List<EntityBase> entities = new List<EntityBase>();
        private static List<ISaveLoadData> saveLoadInterfaces = new List<ISaveLoadData>();
        private static void LoadSaveData()
        {
            CurrentGameMode?.LoadSaveData();

            UIManager.LoadData();

            var list = new List<ISaveLoadData>(saveLoadInterfaces);
            foreach (var sl in list)
            {
                sl.OnLoadData();
            }
        }

        private static void Initialize()
        {
            if (!_Initialized)
            {
                // Page "Advance"
                DebugMenu.AddButton(page: "Advance", label: "Clear PlayerPrefs",
                    onClick: (obj) =>
                    {
                        ClearSaveData();
                    },
                    onShow: null
                );

                DebugMenu.onMenuToggle += LockControlByDebugMenu;

                // Save Data
                LoadSaveData(); // Load game manager scope data

                // Text Table / Localization
                TextTable.Init();

                _Initialized = true;
            }
        }
        private static void LockControlByDebugMenu(bool isOn)
        {
            if (isOn)
                LockControl("DebugMenu");
            else
                UnlockControl("DebugMenu");
        }

        #endregion Private

        #region Public
        public static GameManagerRunnerBase RunnerInstance
        {
            get;
            private set;
        }

        public static GameModeBase CurrentGameMode
        {
            get
            {
                return RunnerInstance?.gameMode;
            }
        }

        public static bool RegisterRunner(GameManagerRunnerBase runner)
        {
            if (RunnerInstance == null)
            {
                RunnerInstance = runner;

                // Setup current exist entities
                foreach (var entity in entities)
                {
                    entity.SetupGameMode(runner.gameMode);
                }
            }
            else
            {
                Debug.LogError("Error: Multiple runner exist", runner);
            }
            return RunnerInstance == runner;
        }

        public static void UnregisterRunner(GameManagerRunnerBase runner)
        {
            if (RunnerInstance == runner)
                RunnerInstance = null;
        }

        public static void LockControl(string flag)
        {
            controlLock.AddFlag(flag);
        }

        public static void UnlockControl(string flag)
        {
            controlLock.RemoveFlag(flag);
        }

        public static void BypassInput(string flag, bool bypass)
        {
            if (bypass)
                bypassInputFlag.AddFlag(flag);
            else
                bypassInputFlag.RemoveFlag(flag);
        }

        public static bool IsTouchInput { get; set; } = false;
        public static bool IsBypassOneFrame { get; set; } = false;
        public static bool IsBypass
        {
            get
            {
                return IsBypassOneFrame || !bypassInputFlag.IsEmpty;
            }
        }
        public static void LateUpdate()
        {
            IsTouchInput = false;
            IsBypassOneFrame = false;
        }

        public static void Update()
        {
            // Update UI
            UIManager.Update();

            var updateList = new List<EntityBase>(entities);
            // Update entities
            foreach (var entity in updateList)
            {
                if (entity && entity.enabled && entity.IsUpdate)
                {
                    entity.OnUpdate();
                }
            }

            CurrentGameMode?.OnUpdate();
        }

        public static void ClearSaveData()
        {
            SS.PlayerPrefs.DeleteAll();

            CurrentGameMode?.ClearSaveData();

            UIManager.ClearData();

            var list = new List<ISaveLoadData>(saveLoadInterfaces);
            foreach (var sl in list)
            {
                sl.OnClearData();
            }

            LoadSaveData();
        }

        public static void SaveSaveData(System.Action<bool> onDone)
        {
            CurrentGameMode?.SaveSaveData((b) =>
            {
                UIManager.SaveData();

                var list = new List<ISaveLoadData>(saveLoadInterfaces);
                foreach (var sl in list)
                {
                    sl.OnSaveData();
                }
                onDone?.Invoke(b);
            });
        }
        public static async Task RunnerStartAsync()
        {
            // First, prepare all exist UI
            UIManager.SetupUIRoot();

            // GameManager intialize
            if (!_Initialized)
            {
                var blackScreen = UIManager.GetUIEntity("BlackScreen");
                blackScreen?.Show();

                // Initialize
                Initialize();

                blackScreen?.Hide();
            }

            // GameMode initialize
            if (CurrentGameMode)
                await CurrentGameMode.Intialize();

            var list = new List<EntityBase>(entities);
            foreach (var entity in list)
            {
                entity?.OnRunnerStart();
            }

            // Call restart
            GameManager.Restart();
        }

        public static void Restart()
        {
            CurrentGameMode?.Restart();

            var list = new List<EntityBase>(entities);
            foreach (var entity in list)
            {
                entity?.OnRestart();
            }

            CurrentGameMode?.LateRestart();
        }

        public static void Register(EntityBase entity)
        {
            if (entity == null)
                return;

            entity.SetupGameMode(RunnerInstance?.gameMode);
            entities.Add(entity);

            // interfaces
            ISaveLoadData sl = entity as ISaveLoadData;
            if (sl != null)
            {
                saveLoadInterfaces.Add(sl);
            }
        }

        public static void Unregister(EntityBase entity)
        {
            entities.Remove(entity);
            ISaveLoadData sl = entity as ISaveLoadData;
            if (sl != null)
            {
                saveLoadInterfaces.Remove(sl);
            }
        }
        #endregion Public

    }
}
