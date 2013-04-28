//
// Created by Owen Evans on 27/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import "Photo.h"


@implementation Photo {

}
-(void)setValue:(id)value forUndefinedKey:(NSString *)key {
    if(![key isEqualToString:@"photos"]){
        DDLogError(@"unable to set key %@ from value %@", key,value);
    }
}


- (NSURL *)fullURL {
    return [NSURL URLWithString:[NSString stringWithFormat:@"%@%@",[self rootUrl],[self fullPath]]];

}

+ (NSDictionary *)JSONKeyPathsByPropertyKey {
    DDLogInfo(@"getting key paths");
    return [[super JSONKeyPathsByPropertyKey] mtl_dictionaryByAddingEntriesFromDictionary: @{
            @"creationTime": @"creation_time",
            @"fullPath":@"full_url",
            @"thumbnailPath":@"thumbnail_url",
            @"rootUrl":@"root_url"
    }];
}

- (NSURL *)thumbnailURL {
    return [NSURL URLWithString:[NSString stringWithFormat:@"%@%@",[self rootUrl],[self thumbnailPath]]];
}

@end