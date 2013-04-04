using System;
using System.IO;
using Camera.Model;
using TinyMessenger;

namespace Camera.Helpers
{
    public class StateManager : IStateManager
    {

        
        public StateManager()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "usnapus.sqlite");
            lock (_dbLock)
            {
                if (Db == null)
                {
                    Db = new SQLiteConnection(dbPath);
                }
                DoMigrations(Db);
            }
        }


        DeviceRegistration _currentDeviceRegistration;

        public DeviceRegistration CurrentDeviceRegistration
        {
            set
            {
                lock (_dbLock)
                {
                    if (_currentDeviceRegistration != null)
                    {
                        Db.Delete(_currentDeviceRegistration);
                    }
                    if (value != null)
                    {
                        Db.Insert(value);
                        Server.SetDeviceCredentials(value.Guid, value.Token);
                    }
                    _currentDeviceRegistration = value;
                }
            }
            get { return _currentDeviceRegistration; }
        }

        void RegisterDevice(string deviceName)
        {
            var deviceRegistrationDetails = Db.Find<DeviceRegistration > (reg => true);
            if (deviceRegistrationDetails == null)
            {
                CurrentDeviceRegistration = Server.RegisterDevice(new DeviceRegistration {
                    Guid = Guid.NewGuid().ToString("N"),
                    Name = deviceName
                });
            }
            else
                _currentDeviceRegistration = deviceRegistrationDetails;
        }

        void DoMigrations(SQLiteConnection db)
        {
            db.CreateTable<DeviceRegistration>();
            //db.CreateTable<CurrentEvent>();
            //db.DeleteAll<CurrentEvent>();
            //db.CreateTable<Photo>();
        }

        static IStateManager _stateManager;

        public static IStateManager Current
        {
            get { return _stateManager ?? (_stateManager = new StateManager()); }
            set { _stateManager = value; }
        }

        IServer _server;
        readonly object _dbLock = new object();
        internal static SQLiteConnection Db;
        static ITinyMessengerHub _messageHub;
        ILogger _logger;
        ILocationCoder _locationCoder;
        ILocationManager _locationManager;

        public ITinyMessengerHub MessageHub
        {
            get { return _messageHub ?? (_messageHub = new TinyMessengerHub()); }
            set { _messageHub = value; }
        }

        public event EventHandler<EventArgs> Authenticated;

        protected virtual void OnAuthenticated()
        {
            EventHandler<EventArgs> handler = Authenticated;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public IServer Server
        {
            get { return _server ?? (_server = new Server(Logger)); }
            set { _server = value; }
        }

        protected ILogger Logger
        {
            get { return _logger ?? (_logger = new RaygunLogger()); }
            set { _logger = value; }
        }



        public string DeviceName
        {
            set
            {
                RegisterDevice(value);
            }
        }

        public bool IsAuthenticated { get { return !String.IsNullOrEmpty(CurrentDeviceRegistration.FacebookId); } }
        public ILocationCoder LocationCoder { get { return _locationCoder ?? (_locationCoder = new LocationCoder()); } set { _locationCoder = value; } }


        public void UpdateDeviceRegistration(string name, string email, string facebookId)
        {

            if (_currentDeviceRegistration != null)
            {
                lock (_dbLock)
                {
                    _currentDeviceRegistration.Name = name;
                    _currentDeviceRegistration.Email = email;
                    _currentDeviceRegistration.FacebookId = facebookId;
                    var savedDevice = Server.RegisterDevice(_currentDeviceRegistration);
                    if (savedDevice != null)
                    {
                        savedDevice.Id = _currentDeviceRegistration.Id;
                        CurrentDeviceRegistration = savedDevice;
                        //Db.Insert(_currentDeviceRegistration);
                    }
                }
            }
            else
            {
                CurrentDeviceRegistration = Server.RegisterDevice(new DeviceRegistration {
                    Email = email,
                    Name = name,
                    FacebookId = facebookId,
                    Guid = Guid.NewGuid().ToString("N")
                });
            }
            if (!string.IsNullOrEmpty(CurrentDeviceRegistration.FacebookId))
            {
                OnAuthenticated();
            }
            Logger.Trace("exit");
        }

        public void InitiateFacebookLogin()
        {
            FacebookSession.Current.InitiateLogin();
        }

        public Event CreateEvent(Event eventToCreate)
        {
            throw new NotImplementedException();
        }

        public Coordinate? CurrentLocation { get; set; }
        public ILocationManager LocationManager
        {
            get { return _locationManager ??(_locationManager= new LocationDelegate()); }
            set { _locationManager = value; }
        }

        public void Dispose()
        {
            lock (_dbLock)
            {
                Logger.Trace("enter");
                if (Db != null)
                {
                    Db.Dispose();
                    Db = null;
                }
            }
            Logger.Trace("exit");
        }
    }

    
}