//
//  AppDelegate.m
//  camera
//
//  Created by Owen Evans on 24/04/13.
//  Copyright (c) 2013 Owen Evans. All rights reserved.
//

#import "AppDelegate.h"
#import "Application.h"
#import "MainMenuViewController.h"
#import "DDASLLogger.h"
#import "DDTTYLogger.h"
#import "TestFlightLogger.h"
#import "TestFlight.h"
#import <Crashlytics/Crashlytics.h>


NSString *const FBSessionStateChangedNotification =
        @"us.usnap.camera:FBSessionStateChangedNotification";

@implementation AppDelegate

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    

    [TestFlight setDeviceIdentifier:[[UIDevice currentDevice] uniqueIdentifier]];

    [TestFlight takeOff:@"36e40e5d-ae3d-47da-bb3a-9d445c52c367"];

    
    [Crashlytics startWithAPIKey:@"076e472d795137f4df4b5d12ec7cea788958e843"];
    
    [DDLog addLogger:[TestFlightLogger sharedInstance]];

    [DDLog addLogger:[DDASLLogger sharedInstance]];
    [DDLog addLogger:[DDTTYLogger sharedInstance]];
    DDLogVerbose(@"loading application");
    

    self.window = [[UIWindow alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
    // Override point for customization after application launch.
    [application setStatusBarHidden:YES];
    DDLogVerbose(@"setting root view controller");
    [[self window] setRootViewController:[[MainMenuViewController alloc] initWithNibName:@"MainMenuView" bundle:[NSBundle mainBundle]]];
    [self.window makeKeyAndVisible];
    [Application initialize];
    DDLogVerbose(@"raising load notification");
    [[NSNotificationCenter defaultCenter] postNotificationName:kApplicationLoadedNotification object:nil];
    DDLogVerbose(@"returning from application did load");

  
    return YES;
}

/*
 * Callback for session changes.
 */
- (void)sessionStateChanged:(FBSession *)session
                      state:(FBSessionState) state
                      error:(NSError *)error
{
    switch (state) {
        case FBSessionStateOpen:
            if (!error) {
                // We have a valid session
                DDLogVerbose(@"User session found");
                [FBRequestConnection startForMeWithCompletionHandler:^(FBRequestConnection *connection, id<FBGraphUser> user, NSError *lookupError) {
                  if(error)
                  {
                      DDLogError(@"error with facebook get user: %@", lookupError);
                      return;
                  }
                    NSString *name = user.name;
                    NSString *email = [NSString stringWithFormat:@"%@",[user objectForKey:@"email"]];
                    NSString *facebookId = user.id;

                    [[Application sharedInstance]loginWithUserId:facebookId Name:name Email:email];

                }];
            }
            break;
        case FBSessionStateClosed:
        case FBSessionStateClosedLoginFailed:
            [FBSession.activeSession closeAndClearTokenInformation];
            break;
        default:
            break;
    }

    [[NSNotificationCenter defaultCenter]
            postNotificationName:FBSessionStateChangedNotification
                          object:session];

    if (error) {
        UIAlertView *alertView = [[UIAlertView alloc]
                initWithTitle:@"Error"
                      message:error.localizedDescription
                     delegate:nil
            cancelButtonTitle:@"OK"
            otherButtonTitles:nil];
        [alertView show];
    }
}

/*
 * Opens a Facebook session and optionally shows the login UX.
 */
- (BOOL)openSessionWithAllowLoginUI:(BOOL)allowLoginUI {
    return [FBSession openActiveSessionWithReadPermissions:nil
                                              allowLoginUI:allowLoginUI
                                         completionHandler:^(FBSession *session,
                                                 FBSessionState state,
                                                 NSError *error) {
                                             [self sessionStateChanged:session
                                                                 state:state
                                                                 error:error];
                                         }];
}

/*
 * If we have a valid session at the time of openURL call, we handle
 * Facebook transitions by passing the url argument to handleOpenURL
 */
- (BOOL)application:(UIApplication *)application
            openURL:(NSURL *)url
  sourceApplication:(NSString *)sourceApplication
         annotation:(id)annotation {
    // attempt to extract a token from the url
    return [FBSession.activeSession handleOpenURL:url];
}

- (void)applicationDidBecomeActive:(UIApplication *)application {
    // We need to properly handle activation of the application with regards to Facebook Login
// (e.g., returning from iOS 6.0 Login Dialog or from fast app switching).
    [FBSession.activeSession handleDidBecomeActive];

}

- (void)applicationWillTerminate:(UIApplication *)application {
    [FBSession.activeSession close];
}

@end
