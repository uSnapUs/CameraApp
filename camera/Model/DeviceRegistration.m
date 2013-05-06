//
// Created by Owen Evans on 24/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import "DeviceRegistration.h"
#import "User.h"


@implementation DeviceRegistration {

}
-(void)setValue:(id)value forUndefinedKey:(NSString *)key {
        DDLogError(@"unable to set key %@ from value %@", key,value);

}

+ (NSValueTransformer *)userJSONTransformer {
    DDLogInfo(@"getting location transformer");
    return [NSValueTransformer mtl_JSONDictionaryTransformerWithModelClass:[User class]];
}
+ (NSDictionary *)JSONKeyPathsByPropertyKey {
    DDLogInfo(@"getting key paths");
    return [[super JSONKeyPathsByPropertyKey] mtl_dictionaryByAddingEntriesFromDictionary: @{
            @"name": @"name",
            @"guid":@"guid",
            @"token":@"token"
    }];
}

@end