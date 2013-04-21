using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Camera.Messages;
using Camera.Model;
using MonoTouch.Foundation;
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
            {
                _currentDeviceRegistration = deviceRegistrationDetails;
                Server.SetDeviceCredentials(_currentDeviceRegistration.Guid, _currentDeviceRegistration.Token);
            }
        }

        void DoMigrations(SQLiteConnection db)
        {
            db.CreateTable<DeviceRegistration>();
            db.DeleteAll<DeviceRegistration>();
            db.DropTable<Event>();
            db.CreateTable<Event>();
            //db.CreateTable<CurrentEvent>();
            //db.DeleteAll<CurrentEvent>();
            db.DropTable<Photo>();
            db.CreateTable<Photo>();
        }

        static IStateManager _stateManager;

        public static IStateManager Current
        {
            get { return _stateManager ?? (_stateManager = new StateManager()); }
            set { _stateManager = value; }
        }

        IServer _server;
        readonly object _dbLock = new object();
        public static SQLiteConnection Db;
        static ITinyMessengerHub _messageHub;
        ILogger _logger;
        ILocationCoder _locationCoder;
        ILocationManager _locationManager;
        readonly List<PhotoUpload> _photoUploads = new List<PhotoUpload>();
        TinyMessageSubscriptionToken _uploadDoneSubscriptionToken;
        TinyMessageSubscriptionToken _uploadProgressToken;
        NSTimer _eventUpdateTimer;

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
            get { return _server ?? (_server = new Server(Logger,MessageHub)); }
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
                        savedDevice.Token = _currentDeviceRegistration.Token;
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
            return Server.CreateEvent(eventToCreate);
        }

        public void UploadPhoto(Event currentEvent,string photoPath)
        {
            if (_uploadDoneSubscriptionToken == null)
            {
                _uploadDoneSubscriptionToken = MessageHub.Subscribe<UploaderDoneMessage>(OnUploadDone);
            }
            if (_uploadProgressToken == null)
            {
                _uploadProgressToken = MessageHub.Subscribe<UploadProgressMessage>(OnUploadProgress);
            }
            var photoUpload = new PhotoUpload {
                Event = currentEvent,
                Path = photoPath
            };
            _photoUploads.Add(photoUpload);
            _server.PostPhoto(currentEvent.Code,photoPath,photoUpload.Id);
        }

        public Photo[] GetEventPhotos(Event ev)
        {
            return (from photo in Db.Table<Photo>() where photo.EventCode ==ev.Code select photo).ToArray();
        }

        public void StartUpdatingPhotosForEvent(Event ev)
        {
            _eventUpdateTimer = NSTimer.CreateRepeatingScheduledTimer(TimeSpan.FromSeconds(30), () => UpdatePhotosForEvent(ev));
            _eventUpdateTimer.Fire();
        }

        public void StopUpdatingPhotosForEvent()
        {
            if (_eventUpdateTimer != null)
            {
                _eventUpdateTimer.Invalidate();
                _eventUpdateTimer.Dispose();
                _eventUpdateTimer = null;
            }
        }

        public void UpdatePhotosForEvent(Event ev)
        {
            var lastUpdateDate = Db.Table<Photo>().Where(p => p.EventCode == ev.Code).OrderByDescending(p=>p.CreationTime).FirstOrDefault();
            DateTime? updateDate = null;
            if (lastUpdateDate != null)
            {
                updateDate = lastUpdateDate.CreationTime;
            }
            var photos = Server.GetPhotos(ev, updateDate);
            if (photos.Length > 0)
            {
                photos = photos.Select(p =>
                    {
                        p.EventCode = ev.Code;
                        return p;
                    }).ToArray();
                Console.WriteLine(photos[0].EventCode);
                Db.InsertAll(photos);
                _messageHub.PublishAsync(new EventPhotoListUpdatedMessage {Sender=this,EventCode = ev.Code});
            }
            
        }

        void OnUploadProgress(UploadProgressMessage obj)
        {
            
        }

        void OnUploadDone(UploaderDoneMessage uploaderDoneMessage)
        {
            if (_eventUpdateTimer != null)
            {
                _eventUpdateTimer.Fire();
            }
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