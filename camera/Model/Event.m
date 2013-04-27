//
// Created by Owen Evans on 26/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import "Event.h"
#import "Location.h"


@implementation Event {

}

-(void)setValue:(id)value forUndefinedKey:(NSString *)key {
    if(![key isEqualToString:@"photos"]){
        DDLogError(@"unable to set key %@ from value %@", key,value);
    }
}

- (CLLocationCoordinate2D)coordinate {
    CLLocationCoordinate2D coordinate2D= CLLocationCoordinate2DMake([[self location] latitude], [[self location] longitude]);
    DDLogVerbose(@"getting coordinate: Latitude %f, longitude %f", coordinate2D.latitude,coordinate2D.longitude);
    return coordinate2D;
}
- (NSString *)title {
    return [self name];
}

+ (NSValueTransformer *)locationJSONTransformer {
    DDLogInfo(@"getting location transformer");
    return [NSValueTransformer mtl_JSONDictionaryTransformerWithModelClass:[Location class]];
}

+ (NSDictionary *)JSONKeyPathsByPropertyKey {
    DDLogInfo(@"getting key paths");
    return [[super JSONKeyPathsByPropertyKey] mtl_dictionaryByAddingEntriesFromDictionary: @{
            @"location": @"location",
            @"name":@"name"
    }];
}


@end