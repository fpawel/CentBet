namespace WebFace

type Global() =
    inherit System.Web.HttpApplication()

    member g.Application_Start(sender: obj, args: System.EventArgs) =
        Betfair.Football.Coupon.start()