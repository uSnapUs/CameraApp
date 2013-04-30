//
//  AppDelegate.h
//  camera
//
//  Created by Owen Evans on 24/04/13.
//  Copyright (c) 2013 Owen Evans. All rights reserved.
//

#import <UIKit/UIKit.h>

#import <FacebookSDK/FacebookSDK.h>

extern NSString *const FBSessionStateChangedNotification;


@interface AppDelegate : UIResponder <UIApplicationDelegate>

@property (strong, nonatomic) UIWindow *window;

- (BOOL)openSessionWithAllowLoginUI:(BOOL)allowLoginUI;

@end
