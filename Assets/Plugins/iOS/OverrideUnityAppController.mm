#import "OverrideUnityAppController.h"

#include "PluginBase/AppDelegateListener.h"


@implementation OverrideUnityAppController

- (BOOL) application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
    [super application:application didFinishLaunchingWithOptions:launchOptions];
    
    if ([launchOptions objectForKey:UIApplicationLaunchOptionsURLKey]) {
        NSURL *url = [launchOptions objectForKey:UIApplicationLaunchOptionsURLKey];
        
        [self performSelector:@selector(openURLAfterDelay:) withObject:url afterDelay:4];
    }
    
    return YES;
}

- (void) openURLAfterDelay:(NSURL*) url
{
    if ([[url absoluteString] rangeOfString:@"miarqui"].location != NSNotFound){
        UnitySendMessage("URLSchemeHandler", "OnOpenWithUrl", [[url absoluteString] UTF8String]);
    }
}

- (BOOL) application:(UIApplication *)application handleOpenURL:(NSURL *)url
{
    if ([[url absoluteString] rangeOfString:@"miarqui"].location != NSNotFound){
        UnitySendMessage("URLSchemeHandler", "OnOpenWithUrl", [[url absoluteString] UTF8String]);
        return YES;
    }
    return NO;
}


- (BOOL) application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation
{
    if ([[url absoluteString] rangeOfString:@"miarqui"].location != NSNotFound){
        UnitySendMessage("URLSchemeHandler", "OnOpenWithUrl", [[url absoluteString] UTF8String]);
        return YES;
    }
    else {
        NSMutableArray* keys    = [NSMutableArray arrayWithCapacity:3];
        NSMutableArray* values    = [NSMutableArray arrayWithCapacity:3];
        
        auto addItem = [&](NSString* key, id value)
        {
            [keys addObject:key];
            if (value == nil) {
                [values addObject:[NSNull null]];
            } else {
                [values addObject:value];
            }
        };
        
        addItem(@"url", url);
        addItem(@"sourceApplication", sourceApplication);
        addItem(@"annotation", annotation);
        
        NSDictionary* notifData = [NSDictionary dictionaryWithObjects:values forKeys:keys];
        AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);
        return YES;
    }
    
    return NO;
}

@end

IMPL_APP_CONTROLLER_SUBCLASS(OverrideUnityAppController)
