//
// Created by Owen Evans on 24/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import "DeviceRegistration.h"

SPEC_BEGIN(DeviceRegistrationSpecifications)

        describe(@"DeviceRegistration", ^{

            describe(@"serialization", ^{

                __block NSDictionary *jsonDictionary;
                beforeAll(^{
                   DeviceRegistration *deviceRegistration = [[DeviceRegistration alloc]init];
                    [deviceRegistration setName:@"name"];
                    [deviceRegistration setGuid:@"guid"];
                   jsonDictionary = [MTLJSONAdapter JSONDictionaryFromModel:deviceRegistration];
                });
                it(@"should serialize name", ^{
                    [[[jsonDictionary objectForKey:@"name"]should] equal:@"name"];
                });
                it(@"should serialize guid",^{
                    [[[jsonDictionary objectForKey:@"guid"]should] equal:@"guid"];
                });
            });
        });

SPEC_END