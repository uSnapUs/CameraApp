//
// Created by Owen Evans on 26/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//

#import "Event.h"
#import "Location.h"

SPEC_BEGIN(EventSpecificaitons)

        describe(@"Event", ^{
           describe(@"deserializing",^{
               __block Event *deserializedEvent;
               __block NSError *error;
               beforeAll(^{
                  NSDictionary *eventDictionary = @{
                          @"name":@"name",
                          @"location":@{
                                  @"type":@"Point",
                                  @"coodinates":@[[NSNumber numberWithDouble:0.4],[NSNumber numberWithDouble:1.3]]
                          }
                  };
                   deserializedEvent = [MTLJSONAdapter modelOfClass:[Event class] fromJSONDictionary:eventDictionary error:&error];
               });
               it(@"should not thow an error", ^{
                   [error shouldBeNil];
               });
               it(@"should create an event",^{
                   [deserializedEvent shouldNotBeNil];
               });
               it(@"should set the name",^{
                   [[[deserializedEvent name]should] equal:@"name"];
               });
               it(@"should set the location",^{
                  [[deserializedEvent location]shouldNotBeNil];
               });
               it(@"should set correct coordinates in location",^{
                   [[theValue([[deserializedEvent location] latitude]) should]equal:theValue(1.3)];
                   [[theValue([[deserializedEvent location] longitude]) should]equal:theValue(0.4)];
               });

           });
        });

SPEC_END