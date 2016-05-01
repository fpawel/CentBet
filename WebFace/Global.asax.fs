namespace WebFace

type Global() =
    inherit System.Web.HttpApplication()

    member g.Application_Start(sender: obj, args: System.EventArgs) =
        #if DEBUG 
        Betfair.Football.Coupon.start()
        #else
        Betfair.Football.Coupon.start()
        #endif