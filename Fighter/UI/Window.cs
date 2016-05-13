using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using ArcheBuddy.Bot.Classes;

namespace Fighter
{
    public partial class Window : Form
    {
        private Host host;
        private Instance instance;

        internal Window(Instance instance)
        {
            InitializeComponent(); this.instance = instance; host = instance.host;
        }


        private bool buttonStartSwitch = false;

        public bool ButtonSwitch
        {
            get
            {
                return buttonStartSwitch;
            }
            set
            {
                buttonStartSwitch = value;
            }
        }

        public void UpdateButtonState(string text, bool state = true)
        {
            Utils.InvokeOn(button_Start, () =>
            {
                button_Start.Text = text;
                button_Start.Enabled = state;
            });
        }

        private void MoveListItem(int direction, ListBox box)
        {
            if (box.SelectedItem != null && box.SelectedIndex >= 0)
            {
                object item = box.SelectedItem;
                int index = box.SelectedIndex, nIndex = (index + direction);

                if (nIndex >= 0 && nIndex < box.Items.Count)
                {
                    box.Items.RemoveAt(index);
                    box.Items.Insert(nIndex, item);

                    box.SetSelected(nIndex, true);
                }
            }
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            if (!buttonStartSwitch)
            {
                instance.moduleBase.Start();
            }
            else
            {
                instance.moduleBase.Stop();
            }
        }

        #region Rotation Controls

        private void button_AddToRotation_Click(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_AllSkills, () =>
            {
                if (lbox_AllSkills.SelectedItem != null)
                {
                    Utils.InvokeOn(lbox_Rotation, () => lbox_Rotation.Items.Add(lbox_AllSkills.SelectedItem));
                }
            });
        }

        private void lbox_Rotation_DoubleClick(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_Rotation, () =>
            {
                if (lbox_Rotation.SelectedItem != null)
                {
                    int index = lbox_Rotation.SelectedIndex;

                    lbox_Rotation.Items.RemoveAt(index);
                }
            });
        }

        private void button_MoveRotationUp_Click(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_Rotation, () => MoveListItem(-1, lbox_Rotation));
        }

        private void button_MoveRotationDown_Click(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_Rotation, () => MoveListItem(1, lbox_Rotation));
        }

        #endregion

        #region Ignored Mobs Controls

        private void button_AddToIgnoredMobs_Click(object sender, EventArgs e)
        {
            Utils.InvokeOn(cmbox_Mobs, () => Utils.InvokeOn(lbox_IgnoredMobs, () =>
            {
                if (cmbox_Mobs.Text != string.Empty && !lbox_IgnoredMobs.Items.Contains(cmbox_Mobs.Text))
                {
                    lbox_IgnoredMobs.Items.Add(cmbox_Mobs.Text);
                }
            }));
        }

        private void lbox_IgnoredMobs_DoubleClick(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_IgnoredMobs, () =>
            {
                if (lbox_IgnoredMobs.SelectedItem != null)
                {
                    int index = lbox_IgnoredMobs.SelectedIndex;

                    lbox_IgnoredMobs.Items.RemoveAt(index);
                }
            });
        }

        private void cmbox_Mobs_DropDown(object sender, EventArgs e)
        {
            PopulateMobs();
        }

        #endregion

        #region PreCombat Buffs Controls

        private void button_AddToPreCombatBuffs_Click(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_AllSkills, () =>
            {
                if (lbox_AllSkills.SelectedItem != null)
                {
                    Utils.InvokeOn(lbox_PreCombatBuffs, () => lbox_PreCombatBuffs.Items.Add(lbox_AllSkills.SelectedItem));
                }
            });
        }

        private void btn_MovePreCombatBuffsUp_Click(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_PreCombatBuffs, () => MoveListItem(-1, lbox_PreCombatBuffs));
        }

        private void btn_MovePreCombatBuffsDown_Click(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_PreCombatBuffs, () => MoveListItem(1, lbox_PreCombatBuffs));
        }

        private void lbox_PreCombatBuffs_DoubleClick(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_PreCombatBuffs, () =>
            {
                if (lbox_PreCombatBuffs.SelectedItem != null)
                {
                    int index = lbox_PreCombatBuffs.SelectedIndex;

                    lbox_PreCombatBuffs.Items.RemoveAt(index);
                }
            });
        }

        #endregion

        #region CombatBuffs Controls

        private void button_AddToCombatBuffs_Click(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_AllSkills, () =>
            {
                if (lbox_AllSkills.SelectedItem != null)
                {
                    Utils.InvokeOn(lbox_CombatBuffs, () => lbox_CombatBuffs.Items.Add(lbox_AllSkills.SelectedItem));
                }
            });

        }

        private void btn_MoveCombatBuffsUp_Click(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_CombatBuffs, () => MoveListItem(-1, lbox_CombatBuffs));
        }

        private void btn_MoveCombatBuffsDown_Click(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_CombatBuffs, () => MoveListItem(1, lbox_CombatBuffs));
        }

        private void lbox_CombatBuffs_DoubleClick(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_CombatBuffs, () =>
            {
                if (lbox_CombatBuffs.SelectedItem != null)
                {
                    int index = lbox_CombatBuffs.SelectedIndex;

                    lbox_CombatBuffs.Items.RemoveAt(index);
                }
            });
        }

        #endregion

        #region Combo Triggers Methods

        private Dictionary<string, List<string>> ComboTriggers = new Dictionary<string, List<string>>();

        private void LoadCombosList(string itemText)
        {
            Utils.InvokeOn(lbox_Combos, () =>
            {
                if (lbox_Combos.Items.Count > 0) lbox_Combos.Items.Clear();

                var combos = ComboTriggers[itemText];

                if (combos != null)
                {
                    lbox_Combos.Items.AddRange(combos.ToArray());
                }
            });
        }

            
        {
            Utils.InvokeOn(lbox_Combos, () => Utils.InvokeOn(lbox_ComboTriggers, () =>
            {
                if (lbox_Combos.SelectedItem != null && lbox_ComboTriggers.SelectedItem != null)
                {
                    string item = lbox_Combos.Text;
                    string keyName = lbox_ComboTriggers.Text;
                    
                    int index = lbox_Combos.SelectedIndex, nIndex = (index + direction);


                    if (nIndex >= 0 && nIndex < ComboTriggers[keyName].Count)
                    {
                        ComboTriggers[keyName].RemoveAt(index);
                        ComboTriggers[keyName].Insert(nIndex, item);

                        MoveListItem(direction, lbox_Combos);
                    }
                }
            }));
        }

        #endregion

        #region Combo Triggers Controls

        private void btn_AddToComboTriggers_Click(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_Rotation, () =>
            {
                if(lbox_Rotation.SelectedItem != null)
                {
                    Utils.InvokeOn(lbox_ComboTriggers, () =>
                    {
                        if(!ComboTriggers.ContainsKey(lbox_Rotation.Text))
                        {
                            lbox_ComboTriggers.Items.Add(lbox_Rotation.SelectedItem); ComboTriggers.Add(lbox_Rotation.Text, new List<string>());
                        }
                    });
                }
            });
        }

        private void lbox_ComboTriggers_SelectedIndexChanged(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_ComboTriggers, () =>
            {
                var selected = lbox_ComboTriggers.SelectedItem;

                if (selected != null)
                {
                    string itemText = lbox_ComboTriggers.Text;
                    LoadCombosList(itemText);
                }
            });
        }

        private void lbox_ComboTriggers_DoubleClick(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_ComboTriggers, () =>
            {
                if (lbox_ComboTriggers.SelectedItem != null)
                {
                    string keyName = lbox_ComboTriggers.Text;
                    int index = lbox_ComboTriggers.SelectedIndex;


                    lbox_ComboTriggers.Items.RemoveAt(index);

                    if (ComboTriggers.ContainsKey(keyName)) ComboTriggers.Remove(keyName);


                    Utils.InvokeOn(lbox_Combos, () => lbox_Combos.Items.Clear());
                }
            });
        }

        private void btn_MoveComboTriggersUp_Click(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_ComboTriggers, () => MoveListItem(-1, lbox_ComboTriggers));
        }

        private void btn_MoveComboTriggersDown_Click(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_ComboTriggers, () => MoveListItem(1, lbox_ComboTriggers));
        }

        #endregion

        #region Combos Controls

        private void button_AddToCombos_Click(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_ComboTriggers, () => Utils.InvokeOn(lbox_AllSkills, () =>
            {
                if(lbox_ComboTriggers.SelectedItem != null && lbox_AllSkills.SelectedItem != null)
                {
                    string itemText = lbox_ComboTriggers.Text;

                    if(ComboTriggers[itemText] != null)
                    {
                        //if (ComboTriggers[itemText].Contains(lbox_AllSkills.Text)) return;

                        ComboTriggers[itemText].Add(lbox_AllSkills.Text);
                        Utils.InvokeOn(lbox_Combos, () => lbox_Combos.Items.Add(lbox_AllSkills.SelectedItem));
                    }
                }
            }));
        }

        private void lbox_Combos_DoubleClick(object sender, EventArgs e)
        {
            Utils.InvokeOn(lbox_Combos, () => Utils.InvokeOn(lbox_ComboTriggers, () =>
            {
                if(lbox_Combos.SelectedItem != null && lbox_ComboTriggers.SelectedItem != null)
                {
                    string keyName = lbox_ComboTriggers.Text;
                    int index = lbox_Combos.SelectedIndex;

                    lbox_Combos.Items.RemoveAt(index); ComboTriggers[keyName].RemoveAt(index);
                }
            }));
        }

        private void button_MoveCombosUp_Click(object sender, EventArgs e)
        {
            CombosMove(-1);
        }

        private void button_MoveCombosDown_Click(object sender, EventArgs e)
        {
            CombosMove(1);
        }

        #endregion

        #region Template Controls

        private string GetTemplateName()
        {
            string templateName = string.Empty;
            Utils.InvokeOn(cmbox_Templates, () => templateName = cmbox_Templates.Text);

            return templateName;
        }

        private void btn_SaveTemplate_Click(object sender, EventArgs e)
        {
            string templateName = GetTemplateName();
            if (templateName.Length < 1) return;


            string templatePath = Paths.TemplatesFolder + templateName + ".template";

            Serializer.Save(FetchTemplate(), templatePath);


            PopulateTemplates();
            Utils.InvokeOn(cmbox_Templates, () => cmbox_Templates.SelectedIndex = cmbox_Templates.Items.IndexOf(templateName));
        }

        private void btn_LoadTemplate_Click(object sender, EventArgs e)
        {
            LoadCombatTemplate();
        }

        private void btn_ImportTemplate_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Combat Templates (*.template)|*.template";
            
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = Path.GetFileName(dialog.FileName),
                      _fileName = Path.GetFileNameWithoutExtension(dialog.FileName);
                       

                if(File.Exists(dialog.FileName))
                {
                    DialogResult importDialog = MessageBox.Show("The template with that name already exists, override: " + _fileName + "?", "Import warning", MessageBoxButtons.YesNo);
                    if (importDialog == DialogResult.No) return;
                }


                if(Serializer.Validate(new CombatTemplate(), dialog.FileName))
                {
                    bool isCopied = false;

                    try
                    {
                        File.Copy(dialog.FileName, Paths.TemplatesFolder + fileName, true);
                        isCopied = true;
                    }
                    catch { }


                    if(isCopied)
                    {
                        Utils.InvokeOn(cmbox_Templates, () =>
                        {
                            PopulateTemplates();


                            int index = cmbox_Templates.Items.IndexOf(_fileName);

                            if(index != -1)
                            {
                                cmbox_Templates.SelectedIndex = index;
                            }
                        });
                    }
                }
            }
        }

        private void btn_DeleteTemplate_Click(object sender, EventArgs e)
        {
            string templateName = GetTemplateName();
            if (templateName.Length < 1) return;


            string templatePath = Paths.TemplatesFolder + templateName + ".template";

            if(!File.Exists(templatePath))
            {
                MessageBox.Show("Template with that name does not exist!");
                return;
            }


            DialogResult deleteDialog = MessageBox.Show("Are you sure you want to delete the template: " + templateName + "?", "Delete warning", MessageBoxButtons.YesNo);
            if (deleteDialog == DialogResult.No) return;


            bool isDeleted = false;

            try
            {
                File.Delete(templatePath);
                isDeleted = true;
            }
            catch { }


            if(isDeleted)
            {
                Utils.InvokeOn(cmbox_Templates, () =>
                {
                    cmbox_Templates.Items.Remove(templateName);
                    if (cmbox_Templates.Items.Count > 0) cmbox_Templates.SelectedIndex = 0;
                });
            }
        }

        #endregion


        // Window Loaded
        public void FormLoaded()
        {
            PopulateControls();

            LoadWindowSettings(); LoadCombatTemplate();


            // Register events
            host.onNewSkillLearned += onNewSkillLearned;
        }

        public void PopulateControls()
        {
            PopulateAllSkills(); PopulateMobs(); PopulateTemplates();
        }
        
        
        public void LoadWindowSettings()
        {
            Settings settings = (Settings)Serializer.Load(new Settings(), Paths.SettingsFolder + host.serverName() + "[" + host.accountId + "].xml");
            if (settings == null) return;

            
            // Load Settings
            Utils.InvokeOn(this, () =>
            {
                Top = settings.windowTop; Left = settings.windowLeft;
            });

            Utils.InvokeOn(cmbox_Templates, () =>
            {
                int index = cmbox_Templates.Items.IndexOf(settings.templateName);

                if(index != -1)
                {
                    cmbox_Templates.SelectedIndex = index;
                }
            });
            Utils.InvokeOn(lbox_IgnoredMobs, () => lbox_IgnoredMobs.Items.AddRange(settings.ignoredMobs.ToArray()));
            Utils.InvokeOn(num_ZoneRadius, () => num_ZoneRadius.Value = settings.zoneRadius);
            Utils.InvokeOn(chkbox_Loot, () => chkbox_Loot.Checked = settings.lootEnabled);
            Utils.InvokeOn(cmbox_Mode, () => cmbox_Mode.SelectedIndex = (int)settings.fightMode);
        }

        public Settings FetchSettings()
        {
            Settings settings = new Settings();


            Utils.InvokeOn(this, () =>
            {
                settings.windowTop = Top; settings.windowLeft = Left;
            });

            Utils.InvokeOn(cmbox_Templates, () => settings.templateName = cmbox_Templates.Text);
            Utils.InvokeOn(lbox_IgnoredMobs, () => settings.ignoredMobs.AddRange(lbox_IgnoredMobs.Items.OfType<string>()));
            Utils.InvokeOn(num_ZoneRadius, () => settings.zoneRadius = (int)num_ZoneRadius.Value);
            Utils.InvokeOn(chkbox_Loot, () => settings.lootEnabled = chkbox_Loot.Checked);
            Utils.InvokeOn(cmbox_Mode, () => settings.fightMode = (FightMode)cmbox_Mode.SelectedIndex);

            return settings;
        }


        public void LoadCombatTemplate()
        {
            string templateName = GetTemplateName();
            if (templateName.Length < 1) return;


            string templatePath = Paths.TemplatesFolder + templateName + ".template";

            if (!File.Exists(templatePath))
            {
                MessageBox.Show("Template with that name does not exist!");
                return;
            }


            CombatTemplate template = (CombatTemplate)Serializer.Load(new CombatTemplate(), templatePath);
            if (template == null) return; ResetTemplate();


            // Load Template
            Utils.InvokeOn(lbox_Rotation, () => lbox_Rotation.Items.AddRange(template.skills.rotation.ToArray()));
            
            foreach(var combos in template.skills.combos)
            {
                ComboTriggers.Add(combos.triggerName, combos.combo);
                Utils.InvokeOn(lbox_ComboTriggers, () => lbox_ComboTriggers.Items.Add(combos.triggerName));
            }

            Utils.InvokeOn(lbox_PreCombatBuffs, () => lbox_PreCombatBuffs.Items.AddRange(template.buffs.preCombat.ToArray()));
            Utils.InvokeOn(lbox_CombatBuffs, () => lbox_CombatBuffs.Items.AddRange(template.buffs.combat.ToArray()));
        }

        public CombatTemplate FetchTemplate()
        {
            CombatTemplate template = new CombatTemplate();


            Utils.InvokeOn(lbox_Rotation, () => template.skills.rotation.AddRange(lbox_Rotation.Items.OfType<string>()));

            foreach (KeyValuePair<string, List<string>> combo in ComboTriggers)
            {
                template.skills.combos.Add(new Combos() { triggerName = combo.Key, combo = combo.Value });
            }

            Utils.InvokeOn(lbox_PreCombatBuffs, () => template.buffs.preCombat.AddRange(lbox_PreCombatBuffs.Items.OfType<string>()));
            Utils.InvokeOn(lbox_CombatBuffs, () => template.buffs.combat.AddRange(lbox_CombatBuffs.Items.OfType<string>()));


            return template;
        }

        private void ResetTemplate()
        {
            Utils.InvokeOn(lbox_Rotation, () => lbox_Rotation.Items.Clear());

            ComboTriggers.Clear();
            Utils.InvokeOn(lbox_ComboTriggers, () => lbox_ComboTriggers.Items.Clear());
            Utils.InvokeOn(lbox_Combos, () => lbox_Combos.Items.Clear());

            Utils.InvokeOn(lbox_PreCombatBuffs, () => lbox_PreCombatBuffs.Items.Clear());
            Utils.InvokeOn(lbox_CombatBuffs, () => lbox_CombatBuffs.Items.Clear());
        }


        private void PopulateAllSkills()
        {
            var skList = new List<string>();

            var classes = host.me.getAbilities().Where(a => a.active).ToArray();


            foreach (var _class in classes)
            {
                var skills = host.me.getSkills().Where(s => s.db.abilityId == (int)_class.id).ToArray();

                if (skills.Length > 0)
                {
                    foreach (var skill in skills) skList.Add(skill.name);
                }
            }


            if (skList.Count > 0)
            {
                skList.Sort();

                Utils.InvokeOn(lbox_AllSkills, () =>
                {
                    if (lbox_AllSkills.Items.Count > 0)
                    {
                        lbox_AllSkills.Items.Clear();
                    }

                    lbox_AllSkills.Items.AddRange(skList.ToArray());
                });
            }
        }

        private void PopulateMobs()
        {
            var mobs = host.getCreatures().Where(m => host.isAttackable(m)).Select(m => m.name).Distinct();
            if (mobs.Count() < 1) return;

            Utils.InvokeOn(cmbox_Mobs, () =>
            {
                if (cmbox_Mobs.Items.Count > 0)
                {
                    mobs = mobs.Except(cmbox_Mobs.Items.OfType<string>().ToList());
                }

                foreach (var mob in mobs) cmbox_Mobs.Items.Add(mob);

                if (cmbox_Mobs.Items.Count > 0 && cmbox_Mobs.SelectedIndex < 0) cmbox_Mobs.SelectedIndex = 0;
            });
        }

        private void PopulateTemplates()
        {
            var templates = Directory.GetFiles(Paths.TemplatesFolder, "*.template");
            if (templates.Length < 1) return;

            Utils.InvokeOn(cmbox_Templates, () =>
            {
                foreach (var temp in templates)
                {
                    string fileName = Path.GetFileNameWithoutExtension(temp);

                    if(!cmbox_Templates.Items.Contains(fileName)) cmbox_Templates.Items.Add(fileName);
                }

                if (cmbox_Templates.Items.Count > 0) cmbox_Templates.SelectedIndex = 0;
            });
        }


        private void onNewSkillLearned(Creature obj, Effect skill)
        {
            PopulateAllSkills();
        }
    }

    [DesignerCategory("Code")]
    internal class Container : Panel
    {
        public Container()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(Color.Transparent))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
                e.Graphics.DrawRectangle(Pens.LightGray, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
            }
        }
    }
}
