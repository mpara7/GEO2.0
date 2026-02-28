using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.EquivalencePlaneGeometry.Engine.Modules;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal.Models;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.PRs;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;
using GeoInferenceEngine.Knowledges.Imps.Componments;
using GeoInferenceEngine.Knowledges.Models;
using GeoInferenceEngine.Knowledges;
using GeoInferenceEngine.PlaneKnowledges.Knowledges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTool.Infrastructures.ZDI;
using GeoInferenceEngine.DebugTest.CalTests.ExprTests;

namespace GeoInferenceEngine.DebugTest.CalTests.RatioInfoTests
{
    internal class RatioInfoTestContext
    {
        public static Expr a = Expr.FromString("a"), b = Expr.FromString("b"), c = Expr.FromString("c"), d = Expr.FromString("d"),
    e = Expr.FromString("e"),
        f = Expr.FromString("f"), g = Expr.FromString("g");
        public static Expr expr = null, expr2 = null, expr3 = null, expr4 = null, result = null;
        public static GeoEquation equation1 = null, equation2 = null;
        public static Point A, B, C, D, E, F, G, H, I, J, W, X, Y, Z;
        public static Segment AB;
        public static Segment CD;
        public static Segment EF;
        public static Segment GH;
        public static Segment IJ;

        public static Segment WX;
        public static Segment XY;
        public static Segment YZ;

        public static CalAddProcessor adder;
        public static ACalExecutor executor;
        public static FormularBase fbase;
        public static KnowledgeAddProcessor ka;
        public static KnowledgeBase kbase;
        public static void ResetContext()
        {
            ExprPreparer.Init();
            (adder, executor, fbase, kbase) = ZDIContainer();
        }
        public static (CalAddProcessor, ACalExecutor, FormularBase, KnowledgeBase) ZDIContainer()
        {
            Knowledge.InitClassIndex(typeof(Knowledge).Assembly);
            ZDIContainer zDIContainer = new ZDIContainer();
            zDIContainer.SetSingleton<FormularBase>();
            zDIContainer.SetSingleton<CalAddProcessor>();
            zDIContainer.SetSingleton<ExprService>();
            zDIContainer.SetSingleton <EmptyCalExecutor, ACalExecutor>();

            zDIContainer.SetSingleton<KnowledgeBase>();
            zDIContainer.SetSingleton<KnowledgeAddProcessor>();
            zDIContainer.SetSingleton<Logger>();
            zDIContainer.SetSingleton<AppInfo>();
            zDIContainer.SetSingleton<EngineInfo>();

            zDIContainer.SetSingleton(new LoggerConfig() { });
            ka = zDIContainer.Get<KnowledgeAddProcessor>();
            A = new Point("A"); ka.Add(A);
            B = new Point("B"); ka.Add(B);
            C = new Point("C"); ka.Add(C);
            D = new Point("D"); ka.Add(D);
            E = new Point("E"); ka.Add(E);
            F = new Point("F"); ka.Add(F);
            G = new Point("G"); ka.Add(G);
            H = new Point("H"); ka.Add(H);
            I = new Point("I"); ka.Add(I);
            J = new Point("J"); ka.Add(J);

            W = new Point("W"); ka.Add(W);
            X = new Point("X"); ka.Add(X);
            Y = new Point("Y"); ka.Add(Y);
            Z = new Point("Z"); ka.Add(Z);
            AB = new Segment(A, B); ka.Add(AB);
            CD = new Segment(C, D); ka.Add(CD);
            EF = new Segment(E, F); ka.Add(EF);
            GH = new Segment(G, H); ka.Add(GH);
            IJ = new Segment(I, J); ka.Add(IJ);

            WX = new Segment(W, X); ka.Add(WX);
            XY = new Segment(X, Y); ka.Add(XY);
            YZ = new Segment(Y, Z); ka.Add(YZ);
            return (zDIContainer.Get<CalAddProcessor>(),
                zDIContainer.Get<ACalExecutor>(),
                zDIContainer.Get<FormularBase>(),
                zDIContainer.Get<KnowledgeBase>());
        }
    }
}
