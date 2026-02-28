using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.EquivalencePlaneGeometry;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
AutoResetEvent inferenceAutoResetEvent = new AutoResetEvent(false);
AutoResetEvent apiAutoResetEvent = new AutoResetEvent(false);
var zscript = "";
string result = "";
Thread thread = new Thread(() =>
{
    while (true)
    {
        inferenceAutoResetEvent.WaitOne();
        result = EPGApp.RunByZScriptWithResult(zscript, GlobalEngineConfigs.Load("题目求解"));
        apiAutoResetEvent.Set();
    }
});
thread.Start();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapGet("/Solve", () => {
    zscript = "Points:A(0,3) B(0,0) C(3,0) D(3,3) E(3,1.5) P(2,3) Q(4,0)\n正方形 ABCD\n中点 ECD\n点在线上 PAD\n直线的交点 Q,PE,BC\nProve:三角形相似 PDEQCE";
    inferenceAutoResetEvent.Set();
    apiAutoResetEvent.WaitOne();
    return Results.Json(new { result = result });
});
app.MapPost("/Solve", async (HttpContext context) =>
{
    // 读取请求体并解析为动态对象  
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    var data = JsonSerializer.Deserialize<qmodel>(body);
    var zscript = "Points:A(0,3) B(0,0) C(3,0) D(3,3) E(3,1.5) P(2,3) Q(4,0)\n正方形 ABCD\n中点 E,CD\n点在线上 PAD\n直线的交点 Q,PE,BC\nProve:三角形相似 PDEQCE";
    var result = EPGApp.RunByZScriptWithResult(data.zscript, GlobalEngineConfigs.Load("默认解题"));
    // 返回响应  
    return Results.Json(new { result = result });
});

app.Run();
class qmodel
{
    public string zscript { get; set; }
}