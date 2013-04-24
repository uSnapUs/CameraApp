//
// Created by Owen Evans on 24/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <ActiveTouch/ATDatabaseContainer.h>
#import "Application.h"



@implementation Application {


}

static Application *sharedInstance;

-(id) init{
    self = [super init];
    if(!self){
        return nil;
    }
    [self setupNotifications] ;
    return self;
}

- (void)setupNotifications {
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(onApplicationLoaded) name:kApplicationLoadedNotification object:nil];

}
-(void) onApplicationLoaded{
    NSLog(@"Setting up database");
    NSString *dbName = kDBName;
    [[ATDatabaseContainer sharedInstance] openDatabaseWithName:dbName];
    NSLog(@"finished Setting up database");
}
-(void)dealloc {
    [[NSNotificationCenter defaultCenter] removeObserver:self];
}

+(Application*)sharedInstance {
    return sharedInstance;
}

+(void)initialize {
    static BOOL initialized = NO;
    if(!initialized)
    {
        initialized = YES;
        sharedInstance = [[Application alloc] init];
    }
}

@end