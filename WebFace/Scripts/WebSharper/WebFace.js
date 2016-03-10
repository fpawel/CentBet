(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,CentBet,Client,Admin,List,T,UI,Next,Doc,Var,View,Var1,Concurrency,PrintfHelpers,console,Utils,Level,Remoting,AjaxRemotingProvider,AttrProxy,Seq,AttrModule,Coupon,View1,Meetup,ListModel,Date,Collections,MapModule,FSharpSet,BalancedTree,Slice,Operators,MatchFailureException;
 Runtime.Define(Global,{
  CentBet:{
   Client:{
    Admin:{
     Br:Runtime.Field(function()
     {
      var arg20;
      arg20=Runtime.New(T,{
       $:0
      });
      return(Admin.op_SpliceUntyped())(Doc.Element("br",[],arg20));
     }),
     Page:function(isAdmin)
     {
      var varIsAdmin;
      varIsAdmin=Var.Create(isAdmin);
      return Doc.EmbedView(View.Map(function(xs)
      {
       return Doc.Concat(List.append(xs,List.ofArray([Admin.msgPart()])));
      },View.Map(function(_arg1)
      {
       return _arg1?Admin.loginBetfairPart():Admin.loginAdminPart(function()
       {
        return Var1.Set(varIsAdmin,true);
       });
      },varIsAdmin.get_View())));
     },
     doc:function(x)
     {
      return x;
     },
     loginAdminPart:function(onok)
     {
      var varAdminKey,loginAdmin;
      varAdminKey=Var.Create("");
      loginAdmin=Concurrency.Delay(function()
      {
       var clo1,arg10,arg00,arg101;
       clo1=function(_)
       {
        var s;
        s="authorize with "+PrintfHelpers.prettyPrint(_);
        return console?console.log(s):undefined;
       };
       arg10=Var1.Get(varAdminKey);
       clo1(arg10);
       arg00=Admin.varMsg();
       arg101={
        $:1,
        $0:[Runtime.New(Level,{
         $:1
        }),"logining admin.."]
       };
       Var1.Set(arg00,arg101);
       return Concurrency.Bind(AjaxRemotingProvider.Async("WebFace:2",[Var1.Get(varAdminKey)]),function(_arg1)
       {
        var _;
        if(_arg1)
         {
          (Admin.setInfo())("login admin : success");
          onok(null);
          _=Concurrency.Return(null);
         }
        else
         {
          (Admin.setError())("login admin : access denied");
          _=Concurrency.Return(null);
         }
        return _;
       });
      });
      return List.ofArray([(Admin.op_SpliceUntyped())(Doc.PasswordBox(Runtime.New(T,{
       $:0
      }),varAdminKey)),Admin.submitButton("login admin",loginAdmin)]);
     },
     loginBetfairPart:function()
     {
      var varUser,varPass,loginBetfair;
      varUser=Var.Create("");
      varPass=Var.Create("");
      loginBetfair=Concurrency.Delay(function()
      {
       var x;
       (Admin.setInfo())("logining betfair...");
       x=AjaxRemotingProvider.Async("WebFace:0",[Var1.Get(varUser),Var1.Get(varPass)]);
       return Concurrency.Bind(x,function(_arg1)
       {
        var arg0,_,error,x1,arg00;
        if(_arg1.$==1)
         {
          error=_arg1.$0;
          _=[Runtime.New(Level,{
           $:3
          }),error];
         }
        else
         {
          _=[Runtime.New(Level,{
           $:1
          }),"successed!"];
         }
        arg0=_;
        x1={
         $:1,
         $0:arg0
        };
        arg00=Admin.varMsg();
        Var1.Set(arg00,x1);
        return Concurrency.Return(null);
       });
      });
      return List.ofArray([Doc.TextNode("User"),(Admin.op_SpliceUntyped())(Doc.PasswordBox(Runtime.New(T,{
       $:0
      }),varUser)),Doc.TextNode("Pass"),(Admin.op_SpliceUntyped())(Doc.PasswordBox(Runtime.New(T,{
       $:0
      }),varPass)),Admin.submitButton("Login betfair",loginBetfair)]);
     },
     msgPart:Runtime.Field(function()
     {
      var x,arg00,_arg00_;
      x=Admin.varMsg().get_View();
      arg00=function(_arg1)
      {
       var _,text,level,patternInput,fore,back,style;
       if(_arg1.$==1)
        {
         text=_arg1.$0[1];
         level=_arg1.$0[0];
         patternInput=(Level.get_color())(level);
         fore=patternInput[1];
         back=patternInput[0];
         style="background-color : "+PrintfHelpers.toSafe(back)+"; color : "+PrintfHelpers.toSafe(fore)+";";
         _=(Admin.op_SpliceUntyped())(Doc.Element("p",List.ofArray([AttrProxy.Create("style",style)]),List.ofArray([Doc.TextNode(text)])));
        }
       else
        {
         _=Doc.get_Empty();
        }
       return _;
      };
      _arg00_=View.Map(arg00,x);
      return Doc.EmbedView(_arg00_);
     }),
     op_SpliceUntyped:Runtime.Field(function()
     {
      return function(x)
      {
       return Admin.doc(x);
      };
     }),
     setError:Runtime.Field(function()
     {
      var level;
      level=Runtime.New(Level,{
       $:3
      });
      return function(x)
      {
       return Admin.setMsg(level,x);
      };
     }),
     setInfo:Runtime.Field(function()
     {
      var level;
      level=Runtime.New(Level,{
       $:1
      });
      return function(x)
      {
       return Admin.setMsg(level,x);
      };
     }),
     setMsg:function(level,x)
     {
      var arg0,x1,arg00;
      arg0=[level,x];
      x1={
       $:1,
       $0:arg0
      };
      arg00=Admin.varMsg();
      return Var1.Set(arg00,x1);
     },
     submitButton:function(value,onclick)
     {
      var varOn,_do_,x,arg00,_arg00_;
      varOn=Var.Create(false);
      _do_=Concurrency.Delay(function()
      {
       Var1.Set(varOn,true);
       return Concurrency.Bind(onclick,function()
       {
        Var1.Set(varOn,false);
        return Concurrency.Return(null);
       });
      });
      x=varOn.get_View();
      arg00=function(isOn)
      {
       var atrs;
       atrs=Seq.toList(Seq.delay(function()
       {
        return isOn?[AttrProxy.Create("disabled","disabled")]:Seq.empty();
       }));
       return Doc.Button(value,atrs,function()
       {
        return Concurrency.Start(_do_,{
         $:0
        });
       });
      };
      _arg00_=View.Map(arg00,x);
      return Doc.EmbedView(_arg00_);
     },
     varMsg:Runtime.Field(function()
     {
      return Var.Create({
       $:0
      });
     }),
     viewMsg:Runtime.Field(function()
     {
      var x,arg00;
      x=Admin.varMsg().get_View();
      arg00=function(_arg1)
      {
       var _,s,level,patternInput,fore,back;
       if(_arg1.$==1)
        {
         s=_arg1.$0[1];
         level=_arg1.$0[0];
         patternInput=(Level.get_color())(level);
         fore=patternInput[1];
         back=patternInput[0];
         _=Doc.Element("p",List.ofArray([AttrModule.Style("color",fore),AttrModule.Style("background",back)]),List.ofArray([Doc.TextNode(s)]));
        }
       else
        {
         _=Doc.get_Empty();
        }
       return _;
      };
      return View.Map(arg00,x);
     })
    },
    Coupon:{
     Main:function()
     {
      var arg00,_builder_,x,x1,arg002,x2;
      arg00=Concurrency.Delay(function()
      {
       return Concurrency.While(function()
       {
        return true;
       },Concurrency.Delay(function()
       {
        return Concurrency.Bind(Coupon.processCoupon(Var1.Get(Coupon.varInplayOnly())),function()
        {
         return Concurrency.Return(null);
        });
       }));
      });
      Concurrency.Start(arg00,{
       $:0
      });
      _builder_=View.get_Do();
      x=Coupon.varInplayOnly().get_View();
      x1=View1.Bind(function(_arg2)
      {
       return View1.Bind(function(_arg3)
       {
        var value,hasToday,predicate,source,value1,hasInplay;
        value=Seq.isEmpty(_arg3);
        hasToday=!value;
        predicate=function(arg001)
        {
         return Meetup.inplay(arg001);
        };
        source=Seq.filter(predicate,_arg3);
        value1=Seq.isEmpty(source);
        hasInplay=!value1;
        return View1.Const([hasToday,hasInplay,_arg2]);
       },Coupon.meetups().get_View());
      },x);
      arg002=function(_arg4)
      {
       var _,_1,arg20,_2,arg201,arg202,arg203;
       if(_arg4[0])
        {
         if(_arg4[1])
          {
           arg20=List.ofArray([Coupon.table()]);
           _1=Doc.Element("table",Runtime.New(T,{
            $:0
           }),List.ofArray([Doc.Element("tbody",[],arg20)]));
          }
         else
          {
           if(_arg4[2])
            {
             arg201=List.ofArray([Doc.TextNode("\u0424\u0443\u0442\u0431\u043e\u043b\u044c\u043d\u044b\u0435 \u043c\u0430\u0442\u0447\u0438 \u0441\u0435\u0433\u043e\u0434\u043d\u044f \u0435\u0449\u0451 \u043d\u0435 \u043d\u0430\u0447\u0430\u043b\u0438\u0441\u044c")]);
             _2=Doc.Element("h1",[],arg201);
            }
           else
            {
             arg202=List.ofArray([Coupon.table()]);
             _2=Doc.Element("table",Runtime.New(T,{
              $:0
             }),List.ofArray([Doc.Element("tbody",[],arg202)]));
            }
           _1=_2;
          }
         _=_1;
        }
       else
        {
         arg203=List.ofArray([Doc.TextNode("\u0421\u0435\u0433\u043e\u0434\u043d\u044f \u043d\u0435\u0442 \u0444\u0443\u0442\u0431\u043e\u043b\u044c\u043d\u044b\u0445 \u043c\u0430\u0442\u0447\u0435\u0439")]);
         _=Doc.Element("h1",[],arg203);
        }
       return _;
      };
      x2=View.Map(arg002,x1);
      return Doc.EmbedView(x2);
     },
     Meetup:Runtime.Class({},{
      id:function(x)
      {
       return x.game.gameId;
      },
      inplay:function(x)
      {
       return Var1.Get(x.gameInfo).playMinute.$==1;
      },
      notinplay:function(x)
      {
       return Var1.Get(x.gameInfo).playMinute.$==0;
      },
      viewGameInfo:function(x)
      {
       return x.gameInfo.get_View();
      }
     }),
     Menu:function()
     {
      var x,arg00,_arg00_;
      x=Coupon.varInplayOnly().get_View();
      arg00=function(isInplayOnly)
      {
       var mapping,list,arg001;
       mapping=function(x1)
       {
        return x1;
       };
       list=List.ofArray([Doc.Element("a",Seq.toList(Seq.delay(function()
       {
        return Seq.append([AttrProxy.Create("href","#")],Seq.delay(function()
        {
         return Seq.append([AttrModule.Handler("click",function()
         {
          return function()
          {
           return Var1.Set(Coupon.varInplayOnly(),true);
          };
         })],Seq.delay(function()
         {
          return isInplayOnly?[AttrProxy.Create("class","active")]:Seq.empty();
         }));
        }));
       })),List.ofArray([Doc.TextNode("\u0412 \u0438\u0433\u0440\u0435")])),Doc.Element("a",Seq.toList(Seq.delay(function()
       {
        return Seq.append([AttrProxy.Create("href","#")],Seq.delay(function()
        {
         return Seq.append([AttrModule.Handler("click",function()
         {
          return function()
          {
           return Var1.Set(Coupon.varInplayOnly(),false);
          };
         })],Seq.delay(function()
         {
          return!isInplayOnly?[AttrProxy.Create("class","active")]:Seq.empty();
         }));
        }));
       })),List.ofArray([Doc.TextNode("\u0412\u0441\u0435 \u043c\u0430\u0442\u0447\u0438")]))]);
       arg001=List.map(mapping,list);
       return Doc.Concat(arg001);
      };
      _arg00_=View.Map(arg00,x);
      return Doc.EmbedView(_arg00_);
     },
     MenuToday:function()
     {
      var x,arg00,_arg00_;
      x=Coupon.varInplayOnly().get_View();
      arg00=function(isInplayOnly)
      {
       return Doc.Element("li",Seq.toList(Seq.delay(function()
       {
        return!isInplayOnly?[AttrProxy.Create("class","active")]:Seq.empty();
       })),List.ofArray([Doc.Element("a",List.ofArray([AttrProxy.Create("href","#"),AttrModule.Handler("click",function()
       {
        return function()
        {
         return Var1.Set(Coupon.varInplayOnly(),false);
        };
       })]),List.ofArray([Doc.TextNode("\u0412\u0441\u0435 \u043c\u0430\u0442\u0447\u0438")]))]));
      };
      _arg00_=View.Map(arg00,x);
      return Doc.EmbedView(_arg00_);
     },
     addNewGames:function(newGames)
     {
      var source,existedMeetups,mapping,projection,action,source2,source1,source3;
      source=Var1.Get(Coupon.meetups().Var);
      existedMeetups=Seq.toList(source);
      Coupon.meetups().Clear();
      mapping=function(tupledArg)
      {
       var game,gameInfo,hash;
       game=tupledArg[0];
       gameInfo=tupledArg[1];
       hash=tupledArg[2];
       return Runtime.New(Meetup,{
        game:game,
        gameInfo:Var.Create(gameInfo),
        hash:hash
       });
      };
      projection=function(x)
      {
       return Var1.Get(x.gameInfo).order;
      };
      action=function(arg00)
      {
       return Coupon.meetups().Add(arg00);
      };
      source2=List.map(mapping,newGames);
      source1=Seq.append(existedMeetups,source2);
      source3=Seq.sortBy(projection,source1);
      return Seq.iter(action,source3);
     },
     downloadCoupon:function(inplayOnly,requst)
     {
      return AjaxRemotingProvider.Async("WebFace:1",[requst,inplayOnly]);
     },
     meetups:Runtime.Field(function()
     {
      var arg00,arg10;
      arg00=function(arg001)
      {
       return Meetup.id(arg001);
      };
      arg10=Runtime.New(T,{
       $:0
      });
      return ListModel.Create(arg00,arg10);
     }),
     processCoupon:function(inplayOnly)
     {
      return Concurrency.Delay(function()
      {
       var mapping,source,source1,requst;
       mapping=function(m)
       {
        return[m.game.gameId,m.hash];
       };
       source=Var1.Get(Coupon.meetups().Var);
       source1=Seq.map(mapping,source);
       requst=Seq.toList(source1);
       return Concurrency.Bind(Coupon.downloadCoupon(inplayOnly,requst),function(_arg1)
       {
        var updGms,outGms,newGms;
        updGms=_arg1[1];
        outGms=_arg1[2];
        newGms=_arg1[0];
        Coupon.updateCoupon(newGms,updGms,outGms);
        return Concurrency.Return(null);
       });
      });
     },
     row:function(x,inplayOnly)
     {
      var _,tx,tx0,gameInfo,op_SpliceUntyped,ats;
      if(inplayOnly?Var1.Get(x.gameInfo).playMinute.$==0:false)
       {
        _=Doc.get_Empty();
       }
      else
       {
        tx=function(x1)
        {
         var arg20;
         arg20=List.ofArray([x1]);
         return Doc.Element("td",[],arg20);
        };
        tx0=function(x1)
        {
         var arg20;
         arg20=List.ofArray([Doc.TextNode(x1)]);
         return Doc.Element("td",[],arg20);
        };
        gameInfo=function(f)
        {
         var arg10,_arg00_;
         arg10=x.gameInfo.get_View();
         _arg00_=View.Map(f,arg10);
         return tx(Doc.TextView(_arg00_));
        };
        op_SpliceUntyped=function(_arg1)
        {
         return Utils.formatDecimalOption(_arg1);
        };
        ats=Runtime.New(T,{
         $:0
        });
        _=Doc.Element("tr",ats,List.ofArray([gameInfo(function(y)
        {
         var patternInput,page,n;
         patternInput=y.order;
         page=patternInput[0];
         n=patternInput[1];
         return Global.String(page)+"."+Global.String(n);
        }),tx0(x.game.home),tx0(x.game.away),gameInfo(function(y)
        {
         return y.status;
        }),gameInfo(function(y)
        {
         return y.summary;
        }),gameInfo(function(y)
        {
         return op_SpliceUntyped(y.winBack);
        }),gameInfo(function(y)
        {
         return op_SpliceUntyped(y.winLay);
        }),gameInfo(function(y)
        {
         return op_SpliceUntyped(y.drawBack);
        }),gameInfo(function(y)
        {
         return op_SpliceUntyped(y.drawLay);
        }),gameInfo(function(y)
        {
         return op_SpliceUntyped(y.loseBack);
        }),gameInfo(function(y)
        {
         return op_SpliceUntyped(y.loseLay);
        })]));
       }
      return _;
     },
     table:function()
     {
      var x,_arg00_;
      x=Coupon.meetups().get_View();
      _arg00_=function(x1)
      {
       var arg00,arg10,_arg00_1;
       arg00=function(tupledArg)
       {
        var x2,inplayOnly;
        x2=tupledArg[0];
        inplayOnly=tupledArg[1];
        return Coupon.row(x2,inplayOnly);
       };
       View.get_Do();
       arg10=View1.Bind(function(_arg1)
       {
        return View1.Bind(function(_arg2)
        {
         return View1.Const([_arg1,_arg2]);
        },Coupon.varInplayOnly().get_View());
       },x1);
       _arg00_1=View.Map(arg00,arg10);
       return Doc.EmbedView(_arg00_1);
      };
      return Doc.ConvertSeq(_arg00_,x);
     },
     updateCoupon:function(newGms,updGms,outGms)
     {
      var value,_,clo1,arg10,value1,_2,action,clo11,arg101,action1;
      value=newGms.$==0;
      if(!value)
       {
        clo1=function(_1)
        {
         var s;
         s="adding new games "+Global.String(_1);
         return console?console.log(s):undefined;
        };
        arg10=newGms.get_Length();
        clo1(arg10);
        _=Coupon.addNewGames(newGms);
       }
      else
       {
        _=null;
       }
      value1=Seq.isEmpty(outGms);
      if(!value1)
       {
        action=function(arg00)
        {
         return Coupon.meetups().RemoveByKey(arg00);
        };
        Seq.iter(action,outGms);
        clo11=function(_1)
        {
         var s;
         s="removing out games "+Global.String(_1);
         return console?console.log(s):undefined;
        };
        arg101=Seq.length(outGms);
        _2=clo11(arg101);
       }
      else
       {
        _2=null;
       }
      action1=function(tupledArg)
      {
       var gameId,gameInfo,hash,matchValue,_1,x;
       gameId=tupledArg[0];
       gameInfo=tupledArg[1];
       hash=tupledArg[2];
       matchValue=Coupon.meetups().TryFindByKey(gameId);
       if(matchValue.$==1)
        {
         x=matchValue.$0;
         Var1.Set(x.gameInfo,gameInfo);
         _1=void(x.hash=hash);
        }
       else
        {
         _1=null;
        }
       return _1;
      };
      return Seq.iter(action1,updGms);
     },
     varInplayOnly:Runtime.Field(function()
     {
      return Var.Create(true);
     })
    },
    Utils:{
     Level:Runtime.Class({},{
      get_color:function()
      {
       return function(_arg1)
       {
        return _arg1.$==3?["lightgrey","red"]:_arg1.$==2?["white","green"]:_arg1.$==4?["black","yellow"]:_arg1.$==0?["lightgrey","gray"]:["white","navy"];
       };
      }
     }),
     dateTimeToString:function($s)
     {
      var $0=this,$this=this;
      return Global.dateTimeToString($s);
     },
     formatDecimal:function(x)
     {
      return Utils.trimEnd(x.toFixed(6),List.ofArray([48,46]));
     },
     formatDecimalOption:function(_arg1)
     {
      var _,x;
      if(_arg1.$==1)
       {
        x=_arg1.$0;
        _=Utils.formatDecimal(x);
       }
      else
       {
        _="";
       }
      return _;
     },
     getDatePart:function(x)
     {
      var y;
      y=new Date(x.getTime());
      y.setHours(0,0,0,0);
      return y;
     },
     mkids:function(x,getid)
     {
      var mapping,elements,m,elements1,s;
      mapping=function(g)
      {
       return[getid(g),g];
      };
      elements=List.map(mapping,x);
      m=MapModule.OfArray(Seq.toArray(elements));
      elements1=List.map(getid,x);
      s=FSharpSet.New1(BalancedTree.OfSeq(elements1));
      return[s,m];
     },
     trimEnd:function(_,_1)
     {
      var _arg1,_2,_3,s,c,_4,rx,s1,_5,s2,rx1,s3;
      _arg1=[_,_1];
      if(_arg1[0]==="")
       {
        _2="";
       }
      else
       {
        if(_arg1[1].$==1)
         {
          s=_arg1[0];
          _arg1[1];
          c=_arg1[1].$0;
          if(s.charCodeAt(s.length-1)===c)
           {
            _arg1[1].$0;
            rx=_arg1[1];
            s1=_arg1[0];
            _4=Utils.trimEnd(Slice.string(s1,{
             $:0
            },{
             $:1,
             $0:s1.length-2
            }),rx);
           }
          else
           {
            if(_arg1[1].$==1)
             {
              s2=_arg1[0];
              rx1=_arg1[1].$1;
              _5=Utils.trimEnd(s2,rx1);
             }
            else
             {
              _5=Operators.Raise(MatchFailureException.New("C:\\Users\\User\\Documents\\Visual Studio 2015\\Projects\\Betfair\\CentBet\\WebFace\\ClientUtils.fs",9,22));
             }
            _4=_5;
           }
          _3=_4;
         }
        else
         {
          s3=_arg1[0];
          _3=s3;
         }
        _2=_3;
       }
      return _2;
     }
    }
   }
  }
 });
 Runtime.OnInit(function()
 {
  CentBet=Runtime.Safe(Global.CentBet);
  Client=Runtime.Safe(CentBet.Client);
  Admin=Runtime.Safe(Client.Admin);
  List=Runtime.Safe(Global.WebSharper.List);
  T=Runtime.Safe(List.T);
  UI=Runtime.Safe(Global.WebSharper.UI);
  Next=Runtime.Safe(UI.Next);
  Doc=Runtime.Safe(Next.Doc);
  Var=Runtime.Safe(Next.Var);
  View=Runtime.Safe(Next.View);
  Var1=Runtime.Safe(Next.Var1);
  Concurrency=Runtime.Safe(Global.WebSharper.Concurrency);
  PrintfHelpers=Runtime.Safe(Global.WebSharper.PrintfHelpers);
  console=Runtime.Safe(Global.console);
  Utils=Runtime.Safe(Client.Utils);
  Level=Runtime.Safe(Utils.Level);
  Remoting=Runtime.Safe(Global.WebSharper.Remoting);
  AjaxRemotingProvider=Runtime.Safe(Remoting.AjaxRemotingProvider);
  AttrProxy=Runtime.Safe(Next.AttrProxy);
  Seq=Runtime.Safe(Global.WebSharper.Seq);
  AttrModule=Runtime.Safe(Next.AttrModule);
  Coupon=Runtime.Safe(Client.Coupon);
  View1=Runtime.Safe(Next.View1);
  Meetup=Runtime.Safe(Coupon.Meetup);
  ListModel=Runtime.Safe(Next.ListModel);
  Date=Runtime.Safe(Global.Date);
  Collections=Runtime.Safe(Global.WebSharper.Collections);
  MapModule=Runtime.Safe(Collections.MapModule);
  FSharpSet=Runtime.Safe(Collections.FSharpSet);
  BalancedTree=Runtime.Safe(Collections.BalancedTree);
  Slice=Runtime.Safe(Global.WebSharper.Slice);
  Operators=Runtime.Safe(Global.WebSharper.Operators);
  return MatchFailureException=Runtime.Safe(Global.WebSharper.MatchFailureException);
 });
 Runtime.OnLoad(function()
 {
  Coupon.varInplayOnly();
  Coupon.meetups();
  Admin.viewMsg();
  Admin.varMsg();
  Admin.setInfo();
  Admin.setError();
  Admin.op_SpliceUntyped();
  Admin.msgPart();
  Admin.Br();
  return;
 });
}());
