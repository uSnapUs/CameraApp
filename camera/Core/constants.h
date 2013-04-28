#import "DDLog.h"

#define kBaseURL @"http://api.isnap.us"
#define kDBName @"usnapus_db"
#define kApplicationLoadedNotification @"uSnapUs_ApplicationLoadedNotification"
#define kDeviceUpdateNotification @"uSnapUs_DeviceUpdateNotification"
#define kEventFoundNotification @"uSnapUs_EventFoundNotification"
#define kEventsForLocationLookedUp @"uSnapUs_EventsForLocationLookedUpNotification"
#define kEventUpdated @"uSnapUs_EventUpdated"
static const int ddLogLevel = LOG_LEVEL_VERBOSE;