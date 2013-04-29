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


           });
            describe(@"serialzing",^{
                __block NSDictionary *eventDictionary;
                __block NSDate *date = [NSDate date];
                beforeAll(^{
                    Event *event  = [[Event alloc]init];
                    event.name = @"name";
                    event.start_date = date;
                    event.end_date = date;
                    event.location = [[Location alloc]init];
                    event.location.coordinates = @[[NSNumber numberWithDouble:1],[NSNumber numberWithDouble:2]];
                    event.is_public = YES;
                    event.address = @"some address";


                    eventDictionary = [MTLJSONAdapter JSONDictionaryFromModel:event];
                });

                it(@"should create an event dictionary",^{
                    [eventDictionary shouldNotBeNil];
                });
                it(@"should set correct start date",^{
                    [[[eventDictionary objectForKey:@"start_date"]should] equal:date];
                });



            });
        });

SPEC_END