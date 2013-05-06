//
// Created by Owen Evans on 6/05/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import "User.h"


@implementation User {

}
-(void)setValue:(id)value forUndefinedKey:(NSString *)key {
    DDLogError(@"unable to set key %@ from value %@", key,value);
}
+ (NSDictionary *)JSONKeyPathsByPropertyKey {
    DDLogInfo(@"getting key paths");
    return [[super JSONKeyPathsByPropertyKey] mtl_dictionaryByAddingEntriesFromDictionary: @{
@"name": @"name",
            @"facebookId":@"facebook_id",
                    @"email":@"email",
    }];
}
@end