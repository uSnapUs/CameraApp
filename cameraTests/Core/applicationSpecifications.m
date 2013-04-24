//
// Created by Owen Evans on 24/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import "Application.h"

SPEC_BEGIN(ApplicaitonSpecificaitons)

        describe(@"Application", ^{
            describe(@"When initialized", ^{
                it(@"should return a singleton instance",^{
                    [[[Application sharedInstance] should] beIdenticalTo:[Application sharedInstance]];
                });
            });

            /*
            describe(@"When load message received",^{
                beforeAll(^{
                    [Application initialize];

                    [[NSNotificationCenter defaultCenter] postNotificationName:kApplicationLoadedNotification object:nil];
                });



            });
            */

        });

SPEC_END
