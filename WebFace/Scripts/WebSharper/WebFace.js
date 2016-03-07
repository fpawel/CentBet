(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,UI,Next,Var,List,Doc,T,View,CentBet,Client,Admin,Var1,Concurrency,PrintfHelpers,console,Remoting,AjaxRemotingProvider,AttrModule,Coupon,View1,Seq,Meetup,AttrProxy,ListModel,Utils,Date,Collections,MapModule,FSharpSet,BalancedTree,Slice,Operators,MatchFailureException;
 Runtime.Define(Global,{
  CentBet:{
   Client:{
    Admin:{
     Config:function()
     {
      var varSessionToken,varAppKey,varOn,varError,mapping,list,arg20,arg201,arg202,arg203,arg204,_arg10_,_arg20_,arg00;
      varSessionToken=Var.Create("");
      varAppKey=Var.Create("");
      varOn=Var.Create(false);
      varError=Var.Create({
       $:0
      });
      mapping=function(x)
      {
       return x;
      };
      arg20=List.ofArray([Doc.TextNode("session token")]);
      arg201=List.ofArray([Doc.Input(Runtime.New(T,{
       $:0
      }),varSessionToken)]);
      arg202=List.ofArray([Doc.TextNode("developers app key")]);
      arg203=List.ofArray([Doc.Input(Runtime.New(T,{
       $:0
      }),varAppKey)]);
      _arg10_=Runtime.New(T,{
       $:0
      });
      _arg20_=function()
      {
       return null;
      };
      arg204=List.ofArray([Doc.Button("Send",_arg10_,_arg20_)]);
      list=List.ofArray([Doc.Element("p",[],arg20),Doc.Element("p",[],arg201),Doc.Element("p",[],arg202),Doc.Element("p",[],arg203),Doc.Element("p",[],arg204)]);
      arg00=List.map(mapping,list);
      return Doc.Concat(arg00);
     },
     Main:function(isAdmin)
     {
      var varIsAdmin;
      varIsAdmin=Var.Create(isAdmin);
      return Doc.EmbedView(View.Map(function(_arg1)
      {
       return _arg1?Admin.Config():Admin.PromptEnterKey(function()
       {
        return Var1.Set(varIsAdmin,true);
       });
      },varIsAdmin.get_View()));
     },
     PromptEnterKey:function(onSuccess)
     {
      var varAdminKey,varOn,varError,varSubmit,authorize,arg00,arg103,x1,arg104,vAuth,arg002,arg20,arg201,_arg10_,_arg20_,arg003,_arg00_,arg004,arg105,_arg00_1,x2,arg005,_arg00_2;
      varAdminKey=Var.Create("");
      varOn=Var.Create(false);
      varError=Var.Create({
       $:0
      });
      varSubmit=Var.Create(null);
      authorize=function()
      {
       return Concurrency.Delay(function()
       {
        var clo1,arg10,arg101,x;
        clo1=function(_)
        {
         var s;
         s="authorize with "+PrintfHelpers.prettyPrint(_);
         return console?console.log(s):undefined;
        };
        arg10=Var1.Get(varAdminKey);
        clo1(arg10);
        Var1.Set(varOn,true);
        arg101={
         $:0
        };
        Var1.Set(varError,arg101);
        x=AjaxRemotingProvider.Async("WebFace:2",[Var1.Get(varAdminKey)]);
        return Concurrency.Bind(x,function(_arg1)
        {
         var _,error,clo11,arg102;
         Var1.Set(varOn,false);
         if(_arg1.$==1)
          {
           error=_arg1.$0;
           clo11=function(_1)
           {
            var s;
            s="authorization error "+PrintfHelpers.prettyPrint(_1);
            return console?console.log(s):undefined;
           };
           clo11(error);
           arg102={
            $:1,
            $0:error
           };
           Var1.Set(varError,arg102);
           _=Concurrency.Return(null);
          }
         else
          {
           if(console)
            {
             console.log("authorized");
            }
           onSuccess(null);
           _=Concurrency.Return(null);
          }
         return _;
        });
       });
      };
      arg00=function(x)
      {
       var _,arg001;
       if(x==="")
        {
         _=null;
        }
       else
        {
         arg001=authorize(null);
         _=Concurrency.Start(arg001,{
          $:0
         });
        }
       return _;
      };
      arg103=varAdminKey.get_View();
      x1=View.Map(arg00,arg103);
      arg104=varSubmit.get_View();
      vAuth=View.SnapshotOn(null,arg104,x1);
      arg20=List.ofArray([Doc.InputArea(Runtime.New(T,{
       $:0
      }),varAdminKey)]);
      _arg10_=Runtime.New(T,{
       $:0
      });
      _arg20_=function()
      {
       return Var1.Set(varSubmit,null);
      };
      arg201=List.ofArray([Doc.Button("submit",_arg10_,_arg20_)]);
      arg003=function()
      {
       return Doc.get_Empty();
      };
      _arg00_=View.Map(arg003,vAuth);
      arg004=function(_arg2)
      {
       var _,arg202;
       if(_arg2)
        {
         arg202=List.ofArray([Doc.TextNode("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd...")]);
         _=Doc.Element("p",[],arg202);
        }
       else
        {
         _=Doc.get_Empty();
        }
       return _;
      };
      arg105=varOn.get_View();
      _arg00_1=View.Map(arg004,arg105);
      x2=varError.get_View();
      arg005=function(_arg3)
      {
       var _,error;
       if(_arg3.$==1)
        {
         error=_arg3.$0;
         _=Doc.Element("p",List.ofArray([AttrModule.Style("color","red")]),List.ofArray([Doc.TextNode(error)]));
        }
       else
        {
         _=Doc.get_Empty();
        }
       return _;
      };
      _arg00_2=View.Map(arg005,x2);
      arg002=List.ofArray([Doc.Element("p",[],arg20),Doc.Element("p",[],arg201),Doc.EmbedView(_arg00_),Doc.EmbedView(_arg00_1),Doc.EmbedView(_arg00_2)]);
      return Doc.Concat(arg002);
     }
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
      return AjaxRemotingProvider.Async("WebFace:0",[requst,inplayOnly]);
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
              _5=Operators.Raise(MatchFailureException.New("E:\\User\\Docs\\Visual Studio 2015\\Projects\\Betfair\\CentBet\\WebFace\\WebClient\\ClientUtils.fs",9,22));
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
  UI=Runtime.Safe(Global.WebSharper.UI);
  Next=Runtime.Safe(UI.Next);
  Var=Runtime.Safe(Next.Var);
  List=Runtime.Safe(Global.WebSharper.List);
  Doc=Runtime.Safe(Next.Doc);
  T=Runtime.Safe(List.T);
  View=Runtime.Safe(Next.View);
  CentBet=Runtime.Safe(Global.CentBet);
  Client=Runtime.Safe(CentBet.Client);
  Admin=Runtime.Safe(Client.Admin);
  Var1=Runtime.Safe(Next.Var1);
  Concurrency=Runtime.Safe(Global.WebSharper.Concurrency);
  PrintfHelpers=Runtime.Safe(Global.WebSharper.PrintfHelpers);
  console=Runtime.Safe(Global.console);
  Remoting=Runtime.Safe(Global.WebSharper.Remoting);
  AjaxRemotingProvider=Runtime.Safe(Remoting.AjaxRemotingProvider);
  AttrModule=Runtime.Safe(Next.AttrModule);
  Coupon=Runtime.Safe(Client.Coupon);
  View1=Runtime.Safe(Next.View1);
  Seq=Runtime.Safe(Global.WebSharper.Seq);
  Meetup=Runtime.Safe(Coupon.Meetup);
  AttrProxy=Runtime.Safe(Next.AttrProxy);
  ListModel=Runtime.Safe(Next.ListModel);
  Utils=Runtime.Safe(Client.Utils);
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
  return;
 });
}());
