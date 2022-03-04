using System.Windows.Forms;
using System;
using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;
using System.Drawing;
using SotnApi;
using System.Text;
using System.Collections.Generic;
using SotnApi.Models;
using SotnApi.Constants.Addresses;
using System.Runtime.InteropServices;

[ExternalTool("Custom-Sotn-Tool")]
public class CustomSotn : ToolFormBase, IExternalToolForm
{
    public ApiContainer? _maybeAPIContainer { get; set; }
    private ApiContainer APIs => _maybeAPIContainer!;

    protected override string WindowTitleStatic => "Custom-Sotn-Tool";

    private Label enemyDebugBar;
    private AlucardApi alucard;
    private ActorApi actorApi;
    private StringBuilder enemyDebugBuilder;
    private Timer enemyDebugTimer;
    private List<LiveActor> liveActors;
    private object lockObj = new object();
    private Button button1;
    private TextBox addressInputField;
    private Label label1;
    private Button button2;
    private List<string> ids;

    private List<byte> cachedActor;



    public CustomSotn()
    {
        InitializeComponent();

        enemyDebugBuilder = new StringBuilder();
    }

    private void OnGetEnemiesTimerTick(object sender, EventArgs e)
    {
        var allActors = actorApi.GetAllActors();

        lock (lockObj)
        {
            liveActors.Clear();
            enemyDebugBuilder.Clear();
            enemyDebugBuilder.AppendLine("Current enemies:");

            foreach (var actorAddress in allActors)
            {
                LiveActor actor = new LiveActor(actorAddress, APIs.Memory);

                if (actor.Hp == 32767)
                    continue;

                liveActors.Add(actor);
                enemyDebugBuilder.AppendLine($"[{actorAddress}] : [HP:{actor.Hp}] : [{actor.Xpos}:{actor.Ypos}] : [id:{actor.Sprite}]");
            }

            enemyDebugBar.Text = enemyDebugBuilder.ToString();

            LogActorIds();
        }
    }

    private void LogActorIds()
    {
        foreach (var actor in liveActors)
        {
            if (actor.Hp > 0 && actor.Hp < 32000)
            {
                if(!ids.Contains(actor.Sprite.ToString()))
                    ids.Add(actor.Sprite.ToString());
            }
        }

        InfoLogger.Log(ids);
    }

    private void SpawnActor(ushort id)
    {
        Actor actor = new Actor();
        actor.Hp = 500;
        actor.Damage = 5;
        actor.Def = 0;
        actor.Sprite = id;
        actor.Xpos = 100;
        actor.Ypos = 152;

        actorApi.SpawnActor(actor);
    }

    private void SpawnActor(long address, ushort id)
    {
        Actor actor = new Actor(actorApi.GetActor(address));
        actor.Hp = 500;
        actor.Sprite = id;
        actor.Xpos = 100;
        actor.Ypos = 152;

        actorApi.SpawnActor(actor);
    }

    public override void Restart()
    {
        alucard = new AlucardApi(APIs.Memory);
        actorApi = new ActorApi(APIs.Memory);
        enemyDebugTimer = new Timer();
        enemyDebugTimer.Tick += OnGetEnemiesTimerTick;
        enemyDebugTimer.Interval = 2000;
        enemyDebugTimer.Start();
        liveActors = new List<LiveActor>(16);
        ids = InfoLogger.GetLog();
    }

    private void InitializeComponent()
    {
            this.enemyDebugBar = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.addressInputField = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // enemyDebugBar
            // 
            this.enemyDebugBar.AutoSize = true;
            this.enemyDebugBar.Location = new System.Drawing.Point(148, 9);
            this.enemyDebugBar.Name = "enemyDebugBar";
            this.enemyDebugBar.Size = new System.Drawing.Size(57, 13);
            this.enemyDebugBar.TabIndex = 0;
            this.enemyDebugBar.Text = "debug-info";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 51);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(113, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "CacheActor";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.CacheActor);
            // 
            // addressInputField
            // 
            this.addressInputField.Location = new System.Drawing.Point(12, 25);
            this.addressInputField.Name = "addressInputField";
            this.addressInputField.Size = new System.Drawing.Size(100, 20);
            this.addressInputField.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Address here";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 80);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(113, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Spawn Cached";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.SpawnCachedActor);
            // 
            // CustomSotn
            // 
            this.ClientSize = new System.Drawing.Size(428, 333);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.addressInputField);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.enemyDebugBar);
            this.Name = "CustomSotn";
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    private void CacheActor(object sender, EventArgs e)
    {
        if (long.TryParse(addressInputField.Text, out long address))
            cachedActor = actorApi.GetActor(address);
    }

    private void SpawnCachedActor(object sender, EventArgs e)
    {
        if(cachedActor != null)
        {
            Actor actor = new Actor(cachedActor);
            actor.Hp = 500;
            actorApi.SpawnActor(actor);
        }
    }
}