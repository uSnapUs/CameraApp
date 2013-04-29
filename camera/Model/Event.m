//
// Created by Owen Evans on 26/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import "Event.h"
#import "Location.h"
#import "Photo.h"


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
+ (NSValueTransformer *)photosJSONTransformer {
    DDLogInfo(@"getting photo transformer");
    return [NSValueTransformer mtl_JSONArrayTransformerWithModelClass:[Photo class]];
}

+ (NSValueTransformer *)end_dateJSONTransformer{
    DDLogVerbose(@"getting end_date transformer");
    return [MTLValueTransformer reversibleTransformerWithForwardBlock:^(NSString *str) {
        return [self.dateFormatter dateFromString:str];
    } reverseBlock:^(NSDate *date) {
        return [self.dateFormatter stringFromDate:date];
    }];
}
+ (NSValueTransformer *)start_dateJSONTransformer{
    DDLogVerbose(@"getting start_date transformer");
    return [MTLValueTransformer reversibleTransformerWithForwardBlock:^(NSString *str) {
        return [self.dateFormatter dateFromString:str];
    } reverseBlock:^(NSDate *date) {
        return [self.dateFormatter stringFromDate:date];
    }];
}

+ (NSValueTransformer *)updatedAtJSONTransformer{
    DDLogVerbose(@"getting updatedAt transformer");
    return [MTLValueTransformer reversibleTransformerWithForwardBlock:^(NSString *str) {
        return [self.dateFormatter dateFromString:str];
    } reverseBlock:^(NSDate *date) {
        return [self.dateFormatter stringFromDate:date];
    }];
}

+ (NSValueTransformer *)createdAtJSONTransformer{
    DDLogVerbose(@"getting createdAt transformer");
    return [MTLValueTransformer reversibleTransformerWithForwardBlock:^(NSString *str) {
        return [self.dateFormatter dateFromString:str];
    } reverseBlock:^(NSDate *date) {
        return [self.dateFormatter stringFromDate:date];
    }];
}

+ (NSValueTransformer *)creation_date_utcJSONTransformer{
    DDLogVerbose(@"getting createdAt transformer");
    return [MTLValueTransformer reversibleTransformerWithForwardBlock:^(NSString *str) {
        return [self.dateFormatter dateFromString:str];
    } reverseBlock:^(NSDate *date) {
        return [self.dateFormatter stringFromDate:date];
    }];
}


+ (NSDateFormatter *)dateFormatter {
    NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
    dateFormatter.locale = [[NSLocale alloc] initWithLocaleIdentifier:@"en_US_POSIX"];
    dateFormatter.dateFormat = @"yyyy-MM-dd'T'HH:mm:ssZZZ";
    return dateFormatter;
}

+ (NSDictionary *)JSONKeyPathsByPropertyKey {
    DDLogInfo(@"getting key paths");
    return [[super JSONKeyPathsByPropertyKey] mtl_dictionaryByAddingEntriesFromDictionary: @{
            @"location": @"location",
            @"name":@"name",
            @"start_date":@"start_date",
            @"end_date":@"end_date"
    }];
}


@end