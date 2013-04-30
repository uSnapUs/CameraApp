//
// Created by Owen Evans on 24/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import "DeviceRegistration.h"


@implementation DeviceRegistration {

}



+ (NSDictionary *)JSONKeyPathsByPropertyKey {
    DDLogInfo(@"getting key paths");
    return [[super JSONKeyPathsByPropertyKey] mtl_dictionaryByAddingEntriesFromDictionary: @{
            @"name": @"name",
            @"email":@"email",
            @"facebookId":@"facebook_id",
            @"guid":@"guid",
            @"token":@"token"
    }];
}

@end