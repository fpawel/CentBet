(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,JSON,PrintfHelpers,window,List,CentBet,Client,Admin,UI,Next,Doc,AttrModule,T,AttrProxy,Seq,Key,Var,Concurrency,Var1,View,Level,Strings,Seq1,Remoting,AjaxRemotingProvider,Unchecked,Storage1,Json,Provider,Id,ListModel,Coupon,View1,Meetup,Utils,console,Date,Collections,MapModule,FSharpSet,BalancedTree,Slice,Operators,MatchFailureException;
 Runtime.Define(Global,{
  CentBet:{
   Client:{
    Admin:{
     Level:Runtime.Class({
      get_s1:function()
      {
       return this.$==2?">":"<";
      }
     },{
      get_color:function()
      {
       return function(_arg1)
       {
        return _arg1.$==1?["lightgrey","red"]:_arg1.$==2?["lightgrey","green"]:["white","navy"];
       };
      }
     }),
     LocalStorage:Runtime.Class({
      Get:function(k)
      {
       var _,arg00,value,e,_1;
       try
       {
        arg00=this.strg.getItem(k);
        value=JSON.parse(arg00);
        _={
         $:0,
         $0:value
        };
       }
       catch(e)
       {
        _1=this.strg;
        _={
         $:1,
         $0:"local storage get item "+PrintfHelpers.prettyPrint(k)+" error : "+PrintfHelpers.prettyPrint(e)+", storage "+PrintfHelpers.prettyPrint(_1)
        };
       }
       return _;
      },
      Set:function(k,value)
      {
       var _,value1,e,_1;
       try
       {
        value1=JSON.stringify(value);
        _={
         $:0,
         $0:this.strg.setItem(k,value1)
        };
       }
       catch(e)
       {
        _1=this.strg;
        _={
         $:1,
         $0:"local storage set item "+PrintfHelpers.prettyPrint(k)+" error : "+PrintfHelpers.prettyPrint(e)+", value "+PrintfHelpers.prettyPrint(value)+", storage "+PrintfHelpers.prettyPrint(_1)
        };
       }
       return _;
      }
     },{
      New:function()
      {
       var r;
       r=Runtime.New(this,{});
       r.strg=window.localStorage;
       return r;
      }
     }),
     RenderCommandPrompt:function()
     {
      var arg00;
      arg00=List.ofArray([(Admin.op_SpliceUntyped())(Doc.Element("span",List.ofArray([AttrModule.Style("margin-left","10px")]),Runtime.New(T,{
       $:0
      }))),(Admin.op_SpliceUntyped())(Doc.Element("label",List.ofArray([AttrProxy.Create("for",Admin["cmd-input"]())]),List.ofArray([Doc.TextNode("Input here:")]))),Admin.renderInput()]);
      return Doc.Concat(arg00);
     },
     RenderConsole:function()
     {
      var _arg00_,_arg10_;
      _arg00_=function(r)
      {
       var arg00,arg20;
       arg20=Runtime.New(T,{
        $:0
       });
       arg00=List.ofArray([(Admin.renderRecord())(r),(Admin.op_SpliceUntyped())(Doc.Element("br",[],arg20))]);
       return Doc.Concat(arg00);
      };
      _arg10_=Admin.varConsole().get_View();
      return Doc.ConvertSeq(_arg00_,_arg10_);
     },
     RenderMenu:function()
     {
      return Doc.Concat(List.map(function(x)
      {
       return x;
      },List.ofArray([Doc.Element("a",Seq.toList(Seq.delay(function()
      {
       return Seq.append([AttrProxy.Create("href","#")],Seq.delay(function()
       {
        return[AttrModule.Handler("click",function()
        {
         return function()
         {
          return Admin.varConsole().Clear();
         };
        })];
       }));
      })),List.ofArray([Doc.TextNode("Clear console")])),Doc.Element("a",Seq.toList(Seq.delay(function()
      {
       return Seq.append([AttrProxy.Create("href","#")],Seq.delay(function()
       {
        return[AttrModule.Handler("click",function()
        {
         return function()
         {
          return Admin.varCommandsHistory().Clear();
         };
        })];
       }));
      })),List.ofArray([Doc.TextNode("Clear history")]))])));
     },
     addRecord:function(level,text)
     {
      return Admin.varConsole().Add({
       Id:Key.Fresh(),
       Text:text,
       Level:level
      });
     },
     "cmd-input":Runtime.Field(function()
     {
      return"cmd-input";
     }),
     cmdKey:function(x)
     {
      return x.Id;
     },
     doc:function(x)
     {
      return x;
     },
     op_SpliceUntyped:Runtime.Field(function()
     {
      return function(x)
      {
       return Admin.doc(x);
      };
     }),
     recordKey:function(x)
     {
      return x.Id;
     },
     renderInput:function()
     {
      var varInput,rvFocusInput,varDisableInput,doSend,handleKeyDown,x,arg00,_arg00_;
      varInput=Var.Create("");
      rvFocusInput=Var.Create(null);
      varDisableInput=Var.Create(false);
      doSend=Concurrency.Delay(function()
      {
       Var1.Set(varDisableInput,true);
       return Concurrency.Bind(Admin.send(Var1.Get(varInput)),function()
       {
        Var1.Set(varDisableInput,false);
        Var1.Set(varInput,"");
        return Concurrency.Return(null);
       });
      });
      handleKeyDown=function(_arg2)
      {
       var _,_1,matchValue,_2,cmd,_3,_4,matchValue1,_5,cmd1,_6;
       if(_arg2==="Enter")
        {
         _=Concurrency.Start(doSend,{
          $:0
         });
        }
       else
        {
         if(_arg2==="Up")
          {
           matchValue=Admin.tryGetCommandFromHistory(_arg2==="Up");
           if(matchValue.$==1)
            {
             cmd=matchValue.$0;
             _3={
              $:1,
              $0:cmd
             };
             Admin.varCmd=function()
             {
              return _3;
             };
             _2=Var1.Set(varInput,cmd.Text);
            }
           else
            {
             _2=null;
            }
           _1=_2;
          }
         else
          {
           if(_arg2==="Down")
            {
             matchValue1=Admin.tryGetCommandFromHistory(_arg2==="Up");
             if(matchValue1.$==1)
              {
               cmd1=matchValue1.$0;
               _6={
                $:1,
                $0:cmd1
               };
               Admin.varCmd=function()
               {
                return _6;
               };
               _5=Var1.Set(varInput,cmd1.Text);
              }
             else
              {
               _5=null;
              }
             _4=_5;
            }
           else
            {
             _4=null;
            }
           _1=_4;
          }
         _=_1;
        }
       return _;
      };
      x=varDisableInput.get_View();
      arg00=function(disable)
      {
       return Doc.Input(Seq.toList(Seq.delay(function()
       {
        return Seq.append([AttrProxy.Create("id",Admin["cmd-input"]())],Seq.delay(function()
        {
         return Seq.append(disable?[AttrProxy.Create("disabled","disabled")]:Seq.empty(),Seq.delay(function()
         {
          return Seq.append([AttrModule.Style("width","80%")],Seq.delay(function()
          {
           return Seq.append([AttrModule.CustomVar(rvFocusInput,function(el)
           {
            return function()
            {
             return el.focus();
            };
           },function()
           {
            return{
             $:0
            };
           })],Seq.delay(function()
           {
            var callback;
            callback=function()
            {
             return function(e)
             {
              return handleKeyDown(e.keyIdentifier);
             };
            };
            return[AttrModule.Handler("keydown",callback)];
           }));
          }));
         }));
        }));
       })),varInput);
      };
      _arg00_=View.Map(arg00,x);
      return Doc.EmbedView(_arg00_);
     },
     renderRecord:Runtime.Field(function()
     {
      var arg00;
      arg00=function(r)
      {
       var patternInput,fore,back;
       patternInput=(Level.get_color())(r.Level);
       fore=patternInput[1];
       back=patternInput[0];
       return(Admin.op_SpliceUntyped())(Doc.Element("span",List.ofArray([AttrModule.Style("color",fore),AttrModule.Style("background",back)]),List.ofArray([Doc.TextNode(r.Level.get_s1()+" "+r.Text)])));
      };
      return function(x)
      {
       var _arg00_;
       _arg00_=View.Map(arg00,x);
       return Doc.EmbedView(_arg00_);
      };
     }),
     send:function(inputText)
     {
      return Concurrency.Delay(function()
      {
       var inputText1,a,xs,_,x,_1;
       inputText1=Strings.Trim(inputText);
       Admin.addRecord(Runtime.New(Level,{
        $:2
       }),inputText1);
       xs=Var1.Get(Admin.varCommandsHistory().Var);
       if(Seq.isEmpty(xs))
        {
         _=true;
        }
       else
        {
         x=Seq1.last(xs);
         _=x.Text!==inputText1;
        }
       if(_)
        {
         Admin.varCommandsHistory().Add({
          Id:Key.Fresh(),
          Text:inputText1
         });
         _1=Concurrency.Return(null);
        }
       else
        {
         _1=Concurrency.Return(null);
        }
       a=_1;
       return Concurrency.Combine(a,Concurrency.Delay(function()
       {
        return Concurrency.TryWith(Concurrency.Delay(function()
        {
         return Concurrency.Bind(AjaxRemotingProvider.Async("WebFace:1",[inputText1]),function(_arg1)
         {
          var msg;
          _arg1[0];
          msg=_arg1[1];
          Admin.addRecord(Runtime.New(Level,{
           $:0
          }),msg);
          return Concurrency.Return(null);
         });
        }),function(_arg2)
        {
         Admin.addRecord(Runtime.New(Level,{
          $:1
         }),_arg2.message);
         return Concurrency.Return(null);
        });
       }));
      });
     },
     tryGetCommandFromHistory:function(isnext)
     {
      var xs,count,_,n,_1,_id_,mapping,predicate,source,source1,v,matchValue,_2,_3,n2,_4,n3,_5,_6,n4,_7,n5,_8,_9,n6,_a,n7,_b,_c,n8,_d,n9,arg0;
      xs=Var1.Get(Admin.varCommandsHistory().Var);
      count=Seq.length(xs);
      if(count===0)
       {
        _={
         $:0
        };
       }
      else
       {
        if(Admin.varCmd().$==1)
         {
          _id_=Admin.varCmd().$0.Id;
          mapping=function(n1)
          {
           return function(x)
           {
            return[x,n1];
           };
          };
          predicate=function(tupledArg)
          {
           var _arg1,_id__;
           _arg1=tupledArg[0];
           tupledArg[1];
           _id__=_arg1.Id;
           return Unchecked.Equals(_id__,_id_);
          };
          source=Var1.Get(Admin.varCommandsHistory().Var);
          source1=Seq.mapi(mapping,source);
          v=Seq.tryFind(predicate,source1);
          matchValue=[v,isnext];
          if(matchValue[0].$==1)
           {
            if(matchValue[1])
             {
              matchValue[0].$0[0];
              n2=matchValue[0].$0[1];
              if(count>0?n2<count-1:false)
               {
                n3=matchValue[0].$0[1];
                matchValue[0].$0[0];
                _4=n3+1;
               }
              else
               {
                if(matchValue[0].$==1)
                 {
                  if(matchValue[1])
                   {
                    _6=matchValue[1]?count-1:0;
                   }
                  else
                   {
                    matchValue[0].$0[0];
                    n4=matchValue[0].$0[1];
                    if(count>0?n4>0:false)
                     {
                      n5=matchValue[0].$0[1];
                      matchValue[0].$0[0];
                      _7=n5-1;
                     }
                    else
                     {
                      _7=matchValue[1]?count-1:0;
                     }
                    _6=_7;
                   }
                  _5=_6;
                 }
                else
                 {
                  _5=matchValue[1]?count-1:0;
                 }
                _4=_5;
               }
              _3=_4;
             }
            else
             {
              if(matchValue[0].$==1)
               {
                if(matchValue[1])
                 {
                  _9=matchValue[1]?count-1:0;
                 }
                else
                 {
                  matchValue[0].$0[0];
                  n6=matchValue[0].$0[1];
                  if(count>0?n6>0:false)
                   {
                    n7=matchValue[0].$0[1];
                    matchValue[0].$0[0];
                    _a=n7-1;
                   }
                  else
                   {
                    _a=matchValue[1]?count-1:0;
                   }
                  _9=_a;
                 }
                _8=_9;
               }
              else
               {
                _8=matchValue[1]?count-1:0;
               }
              _3=_8;
             }
            _2=_3;
           }
          else
           {
            if(matchValue[0].$==1)
             {
              if(matchValue[1])
               {
                _c=matchValue[1]?count-1:0;
               }
              else
               {
                matchValue[0].$0[0];
                n8=matchValue[0].$0[1];
                if(count>0?n8>0:false)
                 {
                  n9=matchValue[0].$0[1];
                  matchValue[0].$0[0];
                  _d=n9-1;
                 }
                else
                 {
                  _d=matchValue[1]?count-1:0;
                 }
                _c=_d;
               }
              _b=_c;
             }
            else
             {
              _b=matchValue[1]?count-1:0;
             }
            _2=_b;
           }
          _1=_2;
         }
        else
         {
          _1=count-1;
         }
        n=_1;
        arg0=Seq.nth(n,xs);
        _={
         $:1,
         $0:arg0
        };
       }
      return _;
     },
     varCmd:Runtime.Field(function()
     {
      return{
       $:0
      };
     }),
     varCommandsHistory:Runtime.Field(function()
     {
      var arg00,arg10;
      arg00=function(x)
      {
       return Admin.cmdKey(x);
      };
      arg10=Storage1.LocalStorage("CentBetConsoleCommandsHistory",{
       Encode:(Provider.get_Default().EncodeRecord(undefined,[["Id",Provider.get_Default().EncodeUnion(Key,"$",[[0,[["$0","Item",Id,0]]]]),0],["Text",Id,0]]))(),
       Decode:(Provider.get_Default().DecodeRecord(undefined,[["Id",Provider.get_Default().DecodeUnion(Key,"$",[[0,[["$0","Item",Id,0]]]]),0],["Text",Id,0]]))()
      });
      return ListModel.CreateWithStorage(arg00,arg10);
     }),
     varConsole:Runtime.Field(function()
     {
      var arg00,arg10;
      arg00=function(x)
      {
       return Admin.recordKey(x);
      };
      arg10=Storage1.LocalStorage("CentBetConsole",{
       Encode:(Provider.get_Default().EncodeRecord(undefined,[["Id",Provider.get_Default().EncodeUnion(Key,"$",[[0,[["$0","Item",Id,0]]]]),0],["Text",Id,0],["Level",Provider.get_Default().EncodeUnion(Level,"$",[[0,[]],[1,[]],[2,[]]]),0]]))(),
       Decode:(Provider.get_Default().DecodeRecord(undefined,[["Id",Provider.get_Default().DecodeUnion(Key,"$",[[0,[["$0","Item",Id,0]]]]),0],["Text",Id,0],["Level",Provider.get_Default().DecodeUnion(Level,"$",[[0,[]],[1,[]],[2,[]]]),0]]))()
      });
      return ListModel.CreateWithStorage(arg00,arg10);
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
              _5=Operators.Raise(MatchFailureException.New("C:\\Users\\User\\Documents\\Visual Studio 2015\\Projects\\Betfair\\CentBet\\WebFace\\ClientUtils.fs",9,18));
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
  JSON=Runtime.Safe(Global.JSON);
  PrintfHelpers=Runtime.Safe(Global.WebSharper.PrintfHelpers);
  window=Runtime.Safe(Global.window);
  List=Runtime.Safe(Global.WebSharper.List);
  CentBet=Runtime.Safe(Global.CentBet);
  Client=Runtime.Safe(CentBet.Client);
  Admin=Runtime.Safe(Client.Admin);
  UI=Runtime.Safe(Global.WebSharper.UI);
  Next=Runtime.Safe(UI.Next);
  Doc=Runtime.Safe(Next.Doc);
  AttrModule=Runtime.Safe(Next.AttrModule);
  T=Runtime.Safe(List.T);
  AttrProxy=Runtime.Safe(Next.AttrProxy);
  Seq=Runtime.Safe(Global.WebSharper.Seq);
  Key=Runtime.Safe(Next.Key);
  Var=Runtime.Safe(Next.Var);
  Concurrency=Runtime.Safe(Global.WebSharper.Concurrency);
  Var1=Runtime.Safe(Next.Var1);
  View=Runtime.Safe(Next.View);
  Level=Runtime.Safe(Admin.Level);
  Strings=Runtime.Safe(Global.WebSharper.Strings);
  Seq1=Runtime.Safe(Global.Seq);
  Remoting=Runtime.Safe(Global.WebSharper.Remoting);
  AjaxRemotingProvider=Runtime.Safe(Remoting.AjaxRemotingProvider);
  Unchecked=Runtime.Safe(Global.WebSharper.Unchecked);
  Storage1=Runtime.Safe(Next.Storage1);
  Json=Runtime.Safe(Global.WebSharper.Json);
  Provider=Runtime.Safe(Json.Provider);
  Id=Runtime.Safe(Provider.Id);
  ListModel=Runtime.Safe(Next.ListModel);
  Coupon=Runtime.Safe(Client.Coupon);
  View1=Runtime.Safe(Next.View1);
  Meetup=Runtime.Safe(Coupon.Meetup);
  Utils=Runtime.Safe(Client.Utils);
  console=Runtime.Safe(Global.console);
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
  Admin.varConsole();
  Admin.varCommandsHistory();
  Admin.varCmd();
  Admin.renderRecord();
  Admin.op_SpliceUntyped();
  Admin["cmd-input"]();
  return;
 });
}());
