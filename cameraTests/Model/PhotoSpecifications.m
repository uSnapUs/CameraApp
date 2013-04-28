//
// Created by Owen Evans on 27/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import "Photo.h"

SPEC_BEGIN(PhotoSpecifications)

        describe(@"photo", ^{
            describe(@"url methods", ^{
                __block Photo *sut;
                beforeAll(^{
                    sut = [[Photo alloc]init];
                    [sut setRootUrl:[NSURL URLWithString:@"http://testurl.com"]];
                    [sut setFullPath:@"/full/path.png"];
                    [sut setThumbnailPath:@"/thumbnail/path.png"];

                });
                it(@"should return full path", ^{
                    [[[[sut fullURL] absoluteString]should] equal:[[NSURL URLWithString:@"http://testurl.com/full/path.png"] absoluteString]];
                });
                it(@"should return thumbnail path", ^{
                    [[[[sut thumbnailURL] absoluteString]should] equal:[[NSURL URLWithString:@"http://testurl.com/thumbnail/path.png"] absoluteString]];
                });
            });
        });

SPEC_END