//
// Created by Owen Evans on 24/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <ActiveTouch/ATDatabaseContainer.h>
#import "Application.h"
#import "Server.h"
#import "CouchTouchDBServer.h"

SPEC_BEGIN(ApplicaitonSpecificaitons)




        describe(@"Application", ^{

            describe(@"When initialized", ^{
                __block ATDatabaseContainer *mockDatabase;
                beforeEach(^{
                    mockDatabase=mock([ATDatabaseContainer class]);
                    [ATDatabaseContainer stub:@selector(sharedInstance) andReturn:mockDatabase];

                });
                it(@"should return a singleton instance",^{
                    [[[Application sharedInstance] should] beIdenticalTo:[Application sharedInstance]];
                });
            });


            describe(@"on application load for first time",^{
                __block ATDatabaseContainer *mockDatabase;
                beforeEach(^{
                    mockDatabase=mock([ATDatabaseContainer class]);
                    [ATDatabaseContainer stub:@selector(sharedInstance) andReturn:mockDatabase];
                    Server *mockServer = mock([Server class]);
                    [[Application sharedInstance] setServer:mockServer];
                    [[Application sharedInstance] onApplicationLoaded];

                });
                it(@"should open db", ^{
                    [verify(mockDatabase) openDatabaseWithName:kDBName];
                });



            });


        });

SPEC_END
