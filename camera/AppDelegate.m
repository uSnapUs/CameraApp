//
//  AppDelegate.m
//  camera
//
//  Created by Owen Evans on 24/04/13.
//  Copyright (c) 2013 Owen Evans. All rights reserved.
//

#import "AppDelegate.h"
#import "Application.h"

@implementation AppDelegate

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    self.window = [[UIWindow alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
    // Override point for customization after application launch.
    self.window.backgroundColor = [UIColor whiteColor];
    [self.window makeKeyAndVisible];
    [Application initialize];
    [[NSNotificationCenter defaultCenter] postNotificationName:kApplicationLoadedNotification object:nil];
    return YES;
}

@end
