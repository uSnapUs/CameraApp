//
// Created by Owen Evans on 26/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import "Location.h"


@implementation Location {

}

-(NSString*) type{
    return @"Point";
}

- (double)latitude {
    return [[[self coordinates] objectAtIndex:1]doubleValue];
}

- (double)longitude {
    return [[[self coordinates] objectAtIndex:0] doubleValue];
}

-(void)setValue:(id)value forUndefinedKey:(NSString *)key {
    DDLogError(@"unable to set key %@ from value %@", key,value);
}

@end