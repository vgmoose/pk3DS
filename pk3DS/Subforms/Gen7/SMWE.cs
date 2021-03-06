﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using pk3DS.Core.CTR;
using pk3DS.Core;
using pk3DS.Core.Randomizers;
using pk3DS.Core.Structures;

namespace pk3DS
{
    public partial class SMWE : Form
    {
        public SMWE(lzGARCFile ed, lzGARCFile zd, lzGARCFile wd)
        {
            InitializeComponent();

            PB_DayIcon.Image = Properties.Resources.sun;
            PB_NightIcon.Image = Properties.Resources.moon;
            PB_DayIcon.SizeMode = PictureBoxSizeMode.CenterImage;
            PB_NightIcon.SizeMode = PictureBoxSizeMode.CenterImage;

            font = L_Location.Font;

            speciesList[0] = "(None)";
            locationList = Main.Config.getText(TextName.metlist_000000);
            locationList = getGoodLocationList(locationList);

            nup_spec = new[]
            {
                new [] { NUP_Forme1, NUP_Forme2, NUP_Forme3, NUP_Forme4, NUP_Forme5, NUP_Forme6, NUP_Forme7, NUP_Forme8, NUP_Forme9, NUP_Forme10 },
                new [] { NUP_Forme11, NUP_Forme12, NUP_Forme13, NUP_Forme14, NUP_Forme15, NUP_Forme16, NUP_Forme17, NUP_Forme18, NUP_Forme19, NUP_Forme20 },
                new [] { NUP_Forme21, NUP_Forme22, NUP_Forme23, NUP_Forme24, NUP_Forme25, NUP_Forme26, NUP_Forme27, NUP_Forme28, NUP_Forme29, NUP_Forme30 },
                new [] { NUP_Forme31, NUP_Forme32, NUP_Forme33, NUP_Forme34, NUP_Forme35, NUP_Forme36, NUP_Forme37, NUP_Forme38, NUP_Forme39, NUP_Forme40 },
                new [] { NUP_Forme41, NUP_Forme42, NUP_Forme43, NUP_Forme44, NUP_Forme45, NUP_Forme46, NUP_Forme47, NUP_Forme48, NUP_Forme49, NUP_Forme50 },
                new [] { NUP_Forme51, NUP_Forme52, NUP_Forme53, NUP_Forme54, NUP_Forme55, NUP_Forme56, NUP_Forme57, NUP_Forme58, NUP_Forme59, NUP_Forme60 },
                new [] { NUP_Forme61, NUP_Forme62, NUP_Forme63, NUP_Forme64, NUP_Forme65, NUP_Forme66, NUP_Forme67, NUP_Forme68, NUP_Forme69, NUP_Forme70 },
                new [] { NUP_Forme71, NUP_Forme72, NUP_Forme73, NUP_Forme74, NUP_Forme75, NUP_Forme76, NUP_Forme77, NUP_Forme78, NUP_Forme79, NUP_Forme80 },
                new [] { NUP_WeatherForme1, NUP_WeatherForme2, NUP_WeatherForme3, NUP_WeatherForme4, NUP_WeatherForme5, NUP_WeatherForme6 }
            };
            cb_spec = new[]
            {
                new[] {CB_Enc1, CB_Enc2, CB_Enc3, CB_Enc4, CB_Enc5, CB_Enc6, CB_Enc7, CB_Enc8, CB_Enc9, CB_Enc10},
                new[] {CB_Enc11, CB_Enc12, CB_Enc13, CB_Enc14, CB_Enc15, CB_Enc16, CB_Enc17, CB_Enc18, CB_Enc19, CB_Enc20},
                new[] {CB_Enc21, CB_Enc22, CB_Enc23, CB_Enc24, CB_Enc25, CB_Enc26, CB_Enc27, CB_Enc28, CB_Enc29, CB_Enc30},
                new[] {CB_Enc31, CB_Enc32, CB_Enc33, CB_Enc34, CB_Enc35, CB_Enc36, CB_Enc37, CB_Enc38, CB_Enc39, CB_Enc40},
                new[] {CB_Enc41, CB_Enc42, CB_Enc43, CB_Enc44, CB_Enc45, CB_Enc46, CB_Enc47, CB_Enc48, CB_Enc49, CB_Enc50},
                new[] {CB_Enc51, CB_Enc52, CB_Enc53, CB_Enc54, CB_Enc55, CB_Enc56, CB_Enc57, CB_Enc58, CB_Enc59, CB_Enc60},
                new[] {CB_Enc61, CB_Enc62, CB_Enc63, CB_Enc64, CB_Enc65, CB_Enc66, CB_Enc67, CB_Enc68, CB_Enc69, CB_Enc70},
                new[] {CB_Enc71, CB_Enc72, CB_Enc73, CB_Enc74, CB_Enc75, CB_Enc76, CB_Enc77, CB_Enc78, CB_Enc79, CB_Enc80},
                new[] {CB_WeatherEnc1, CB_WeatherEnc2, CB_WeatherEnc3, CB_WeatherEnc4, CB_WeatherEnc5, CB_WeatherEnc6}
            };
            rate_spec = new[]
            {NUP_Rate1, NUP_Rate2, NUP_Rate3, NUP_Rate4, NUP_Rate5, NUP_Rate6, NUP_Rate7, NUP_Rate8, NUP_Rate9, NUP_Rate10};

            foreach (var cb_l in cb_spec) foreach (var cb in cb_l) { cb.Items.AddRange(speciesList); cb.SelectedIndex = 0; cb.SelectedIndexChanged += updateSpeciesForm; }
            foreach (var nup_l in nup_spec) foreach (var nup in nup_l) { nup.ValueChanged += updateSpeciesForm; }
            foreach (var nup in rate_spec) { nup.Value = 0; nup.ValueChanged += updateEncounterRate; }

            byte[][] zdfiles = zd.Files;
            worldData = zdfiles[1]; // 1.bin
            zoneData = zdfiles[0]; // dec_0.bin
            Zones = new ZoneData7[zoneData.Length / ZoneData7.SIZE];

            var Worlds = wd.Files.Select(f => mini.unpackMini(f, "WD")[0]).ToArray();
            for (int i = 0; i < Zones.Length; i++)
            {
                Zones[i] = new ZoneData7(zoneData, i) {WorldIndex = BitConverter.ToUInt16(worldData, i*0x2)};
                Zones[i].setName(locationList, i);
                var World = Worlds[Zones[i].WorldIndex];
                var mappingOffset = BitConverter.ToInt32(World, 0x8);
                for (var ofs = mappingOffset; ofs < World.Length; ofs += 4)
                {
                    if (BitConverter.ToUInt16(World, ofs) != i)
                        continue;
                    Zones[i].AreaIndex = BitConverter.ToUInt16(World, ofs + 2);
                    break;
                }
            }

            encdata = ed;
            LoadData();
        }

        public static string[] getGoodLocationList(string[] list)
        {
            var bad = list;
            var good = (string[])bad.Clone();
            for (int i = 0; i < bad.Length; i += 2)
            {
                var nextLoc = bad[i + 1];
                if (!string.IsNullOrWhiteSpace(nextLoc) && nextLoc[0] != '[')
                    good[i] += $" ({nextLoc})";
                if (i > 0 && !string.IsNullOrWhiteSpace(good[i]) && good.Take(i - 1).Contains(good[i]))
                    good[i] += $" ({good.Take(i - 1).Count(s => s == good[i]) + 1})";
            }
            return good;
        }

        private Area7[] Areas;
        private readonly ZoneData7[] Zones;
        private readonly lzGARCFile encdata;

        private static readonly string[] speciesList = Main.Config.getText(TextName.SpeciesNames);
        private static string[] locationList;
        private static byte[] zoneData;
        private static byte[] worldData;

        private static Font font;

        private readonly NumericUpDown[][] nup_spec;
        private readonly ComboBox[][] cb_spec;
        private readonly NumericUpDown[] rate_spec;

        private bool loadingdata;

        private EncounterTable CurrentTable;

        private void LoadData()
        {
            loadingdata = true;
            int fileCount = encdata.FileCount;
            var numAreas = fileCount / 11;
            Areas = new Area7[numAreas];
            for (int i = 0; i < numAreas; i++)
            {
                Areas[i] = new Area7
                {
                    FileNumber = 9 + 11*i,
                    Zones = Zones.Where(z => z.AreaIndex == i).ToArray()
                };
                var md = encdata[Areas[i].FileNumber];
                if (md.Length > 0)
                {
                    byte[][] Tables = mini.unpackMini(md, "EA");
                    Areas[i].HasTables = Tables.Any(t => t.Length > 0);
                    if (Areas[i].HasTables)
                    {
                        foreach (var Table in Tables)
                        {
                            var DayTable = Table.Skip(4).Take(0x164).ToArray();
                            var NightTable = Table.Skip(0x168).ToArray();
                            Areas[i].Tables.Add(new EncounterTable(DayTable));
                            Areas[i].Tables.Add(new EncounterTable(NightTable));
                        }
                    }
                }
                else
                {
                    Areas[i].HasTables = false;
                }
            }
            Areas = Areas.OrderBy(a => a.Zones[0].Name).ToArray();

            CB_LocationID.Items.Clear();
            CB_LocationID.Items.AddRange(Areas.Select(a => a.Name).ToArray());

            foreach (Control ctrl in Controls)
                ctrl.Enabled = true;
            B_Randomize.Enabled = true; // Randomization: complete
            CB_SlotRand.SelectedIndex = 0;

            CB_LocationID.SelectedIndex = 0;
            loadingdata = false;
            updateMap(null, null);
        }

        private void DumpTables(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.FileName = "EncounterTables.txt";
                if (sfd.ShowDialog() != DialogResult.OK)
                    return;
                var sb = new StringBuilder();
                foreach (var Map in Areas)
                    sb.Append(Map);
                File.WriteAllText(sfd.FileName, sb.ToString());
            }
        }

        private void updateMap(object sender, EventArgs e)
        {
            loadingdata = true;
            CB_TableID.Items.Clear();
            if (Areas[CB_LocationID.SelectedIndex].HasTables)
            {
                for (int i = 0; i < Areas[CB_LocationID.SelectedIndex].Tables.Count; i += 2)
                {
                    CB_TableID.Items.Add($"{i / 2 + 1} (Day)");
                    CB_TableID.Items.Add($"{i / 2 + 1} (Night)");
                }
            }
            else
                CB_TableID.Items.Add("(None)");
            CB_TableID.SelectedIndex = 0;
            loadingdata = false;
            updatePanel(sender, e);
        }

        private void updatePanel(object sender, EventArgs e)
        {
            if (loadingdata)
                return;
            loadingdata = true;
            var Map = Areas[CB_LocationID.SelectedIndex];
            GB_Encounters.Enabled = Map.HasTables;
            if (!Map.HasTables)
            {
                loadingdata = false;
                return;
            }
            CurrentTable = new EncounterTable(Map.Tables[CB_TableID.SelectedIndex].Data);
            NUP_Min.Value = CurrentTable.MinLevel;
            NUP_Max.Minimum = CurrentTable.MinLevel;
            NUP_Max.Value = CurrentTable.MaxLevel;
            for (int slot = 0; slot < CurrentTable.Encounters.Length; slot++)
            for (int i = 0; i < CurrentTable.Encounters[slot].Length; i++)
            {
                var sl = CurrentTable.Encounters[slot];
                if (slot == 8)
                    sl = CurrentTable.AdditionalSOS;
                rate_spec[i].Value = CurrentTable.Rates[i];
                cb_spec[slot][i].SelectedIndex = (int)sl[i].Species;
                nup_spec[slot][i].Value = (int)sl[i].Forme;
            }
            loadingdata = false;

            int base_id = CB_TableID.SelectedIndex/2;
            base_id *= 2;
            PB_DayTable.Image = Map.Tables[base_id].GetTableImg();
            PB_NightTable.Image = Map.Tables[base_id + 1].GetTableImg();
        }

        private void updateMinMax(object sender, EventArgs e)
        {
            if (loadingdata)
                return;
            loadingdata = true;
            int min = (int) NUP_Min.Value;
            int max = (int) NUP_Max.Value;
            if (max < min)
            {
                max = min;
                NUP_Max.Value = max;
                NUP_Max.Minimum = min;
            }
            CurrentTable.MinLevel = min;
            CurrentTable.MaxLevel = max;
            loadingdata = false;
        }

        private void updateSpeciesForm(object sender, EventArgs e)
        {
            if (loadingdata)
                return;

            var cur_pb = CB_TableID.SelectedIndex%2 == 0 ? PB_DayTable : PB_NightTable;
            var cur_img = cur_pb.Image;
            
            object[][] source = sender is NumericUpDown ? (object[][])nup_spec : cb_spec;
            int table = Array.FindIndex(source, t => t.Contains(sender));
            int slot = Array.IndexOf(source[table], sender);

            var cb_l = cb_spec[table];
            var nup_l = nup_spec[table];
            if (table == 8)
            {
                CurrentTable.AdditionalSOS[slot].Species = (uint)cb_l[slot].SelectedIndex;
                CurrentTable.AdditionalSOS[slot].Forme = (uint)nup_l[slot].Value;
            }
            CurrentTable.Encounters[table][slot].Species = (uint)cb_l[slot].SelectedIndex;
            CurrentTable.Encounters[table][slot].Forme = (uint)nup_l[slot].Value;

            using (var g = Graphics.FromImage(cur_img))
            {
                int x = 40*slot;
                int y = 30*(table + 1);
                if (table == 8)
                {
                    x = 40*slot + 60;
                    y = 270;
                }
                var pnt = new Point(x, y);
                g.SetClip(new Rectangle(pnt.X, pnt.Y, 40, 30), CombineMode.Replace);
                g.Clear(Color.Transparent);

                var enc = CurrentTable.Encounters[table][slot];
                g.DrawImage(enc.Species == 0 ? Properties.Resources.empty : WinFormsUtil.getSprite((int)enc.Species, (int)enc.Forme, 0, 0, Main.Config), pnt);
            }

            cur_pb.Image = cur_img;
        }

        private void updateEncounterRate(object sender, EventArgs e)
        {
            if (loadingdata)
                return;
            
            var cur_pb = CB_TableID.SelectedIndex%2 == 0 ? PB_DayTable : PB_NightTable;
            var cur_img = cur_pb.Image;
            
            int slot = Array.IndexOf(rate_spec, sender);
            int rate = (int) ((NumericUpDown) sender).Value;
            CurrentTable.Rates[slot] = rate;
            
            using (var g = Graphics.FromImage(cur_img))
            {
                var pnt = new PointF(40 * slot + 10, 10);
                g.SetClip(new Rectangle((int) pnt.X, (int) pnt.Y, 40, 14), CombineMode.Replace);
                g.Clear(Color.Transparent);
                g.DrawString($"{rate}%", font, Brushes.Black, pnt);
            }
            
            cur_pb.Image = cur_img;
            
            int tot = 0;
            foreach (var nup in rate_spec) { tot += (int) nup.Value; }
            GB_Encounters.Text = $"Encounters ({tot}%)";
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            int tot = 0;
            foreach (var nup in rate_spec) { tot += (int) nup.Value; }
            
            if (tot != 100 && tot != 0)
            {
                WinFormsUtil.Error("Encounter rates must add up to either 0% or 100%.");
                return;
            }
            
            CurrentTable.Write();
            var area = Areas[CB_LocationID.SelectedIndex];
            area.Tables[CB_TableID.SelectedIndex] = CurrentTable;

            // Set data back to GARC
            encdata[area.FileNumber] = getMapData(area.Tables);
        }

        private void B_Export_Click(object sender, EventArgs e)
        {
            B_Save_Click(sender, e);

            Directory.CreateDirectory("encdata");
            foreach (var Map in Areas)
            {
                var packed = getMapData(Map.Tables);
                File.WriteAllBytes(Path.Combine("encdata", Map.FileNumber.ToString()), packed);
            }
            WinFormsUtil.Alert("Exported all tables!");
        }
        private byte[] getMapData(List<EncounterTable> tables)
        {
            byte[][] tabs = new byte[tables.Count/2][];
            for (int i = 0; i < tables.Count; i += 2)
                tabs[i/2] = new byte[4].Concat(tables[i].Data).Concat(tables[i + 1].Data).ToArray();
            return mini.packMini(tabs, "EA");
        }

        private class Area7
        {
            public string Name => string.Join(" / ", Zones.Select(z => z.Name));
            public int FileNumber;
            public bool HasTables;
            public readonly List<EncounterTable> Tables;
            public ZoneData7[] Zones;

            public Area7()
            {
                Tables = new List<EncounterTable>();
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendLine("==========");
                sb.AppendLine($"Map: {Name}");
                sb.AppendLine($"Tables: {Tables.Count / 2}");
                for (int i = 0; i < Tables.Count / 2; i++)
                {
                    sb.AppendLine($"Table {i+1} (Day):");
                    sb.AppendLine(Tables[i*2].ToString());
                    sb.AppendLine($"Table {i+1} (Night):");
                    sb.AppendLine(Tables[i*2 + 1].ToString());
                }
                sb.AppendLine("==========");
                return sb.ToString();
            }
        }

        private class EncounterTable
        {
            public int MinLevel;
            public int MaxLevel;
            public int[] Rates;
            public readonly Encounter[][] Encounters;
            public readonly Encounter[] AdditionalSOS;

            public readonly byte[] Data;

            public EncounterTable(byte[] t)
            {
                Rates = new int[10];
                Encounters = new Encounter[9][];
                MinLevel = t[0];
                MaxLevel = t[1];
                for (int i = 0; i < Rates.Length; i++)
                    Rates[i] = t[2 + i];
                for (int i = 0; i < Encounters.Length - 1; i++)
                {
                    Encounters[i] = new Encounter[10];
                    var ofs = 0xC + i * 4 * Encounters[i].Length;
                    for (int j = 0; j < Encounters[i].Length; j++)
                    {
                        Encounters[i][j] = new Encounter(BitConverter.ToUInt32(t, ofs + 4 * j));
                    }
                }
                AdditionalSOS = new Encounter[6];
                for (var i = 0; i < AdditionalSOS.Length; i++)
                {
                    AdditionalSOS[i] = new Encounter(BitConverter.ToUInt32(t, 0x14C + 4 * i));
                }
                Encounters[8] = AdditionalSOS;
                Data = (byte[])t.Clone();
            }

            public void Write()
            {
                Data[0] = (byte)MinLevel;
                Data[1] = (byte)MaxLevel;
                for (int i = 0; i < Rates.Length; i++)
                {
                    Data[2 + i] = (byte)Rates[i];
                }
                for (int i = 0; i < Encounters.Length - 1; i++)
                {
                    var ofs = 0xC + i * 4 * Encounters[i].Length;
                    for (int j = 0; j < Encounters[i].Length; j++)
                    {
                        BitConverter.GetBytes(Encounters[i][j].RawValue).CopyTo(Data, ofs + 4 * j);
                    }
                }
                for (int i = 0; i < AdditionalSOS.Length; i++)
                    BitConverter.GetBytes(AdditionalSOS[i].RawValue).CopyTo(Data, 0x14C + 4 * i);
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                for (int i = 0; i < Encounters.Length - 1; i++)
                {
                    var tn = "Encounters";
                    if (i != 0)
                        tn = "SOS Slot " + i;
                    sb.Append($"{tn} (Levels {MinLevel}-{MaxLevel}): ");
                    var specToRate = new Dictionary<uint, int>();
                    var distincts = new List<Encounter>();
                    for (int j = 0; j < Encounters[i].Length; j++)
                    {
                        var encounter = Encounters[i][j];
                        if (!specToRate.ContainsKey(encounter.RawValue))
                        {
                            specToRate[encounter.RawValue] = 0;
                            distincts.Add(encounter);
                        }
                        specToRate[encounter.RawValue] += Rates[j];
                    }
                    distincts = distincts.OrderBy(e => specToRate[e.RawValue]).Reverse().ToList();
                    sb.AppendLine(string.Join(", ", distincts.Select(e => $"{e.ToString()} ({specToRate[e.RawValue]}%)")));
                }
                sb.Append("Additional SOS encounters: ");
                sb.AppendLine(string.Join(", ", AdditionalSOS.Select(e => e.RawValue).Distinct().Select(e => new Encounter(e)).Select(e => e.ToString())));

                return sb.ToString();
            }

            public Bitmap GetTableImg()
            {
                var img = new Bitmap(10*40, 10*30);
                using (var g = Graphics.FromImage(img))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                    for (int i = 0; i < Encounters.Length - 1; i++)
                        for (int j = 0; j < Encounters[i].Length; j++)
                            g.DrawImage(Encounters[i][j].Species == 0 ? Properties.Resources.empty : WinFormsUtil.getSprite((int)Encounters[i][j].Species, (int)Encounters[i][j].Forme, 0, 0, Main.Config), new Point(40 * j, 30 * (i+1)));
                    for (int i = 0; i < Rates.Length; i++)
                        g.DrawString($"{Rates[i]}%", font, Brushes.Black, new PointF(40 * i + 10, 10));
                    g.DrawString("Weather: ", font, Brushes.Black, new PointF(10, 280));
                    for (int i = 0; i < AdditionalSOS.Length; i++)
                        g.DrawImage(AdditionalSOS[i].Species == 0 ? Properties.Resources.empty : WinFormsUtil.getSprite((int)AdditionalSOS[i].Species, (int)AdditionalSOS[i].Forme, 0, 0, Main.Config), new Point(40*i + 60, 270));
                }
                return img;
            }
        }

        private class Encounter
        {
            public uint Species;
            public uint Forme;
            public uint RawValue => Species | (Forme << 11);

            public Encounter(uint val)
            {
                Species = val & 0x7FF;
                Forme = (val >> 11) & 0x1F;
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append(speciesList[Species]);
                if (Forme != 0)
                    sb.Append($" (Forme {Forme})");
                return sb.ToString();
            }
        }

        private void modifyLevels(object sender, EventArgs e)
        {
            if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Modify all current Level ranges?", "Cannot undo.") != DialogResult.Yes) return;

            // Disable Interface while modifying
            Enabled = false;

            // Cycle through each location to modify levels
            foreach (var Table in Areas.SelectMany(Map => Map.Tables))
            {
                Table.MinLevel = Randomizer.getModifiedLevel(Table.MinLevel, NUD_LevelAmp.Value);
                Table.MaxLevel = Randomizer.getModifiedLevel(Table.MaxLevel, NUD_LevelAmp.Value);
                Table.Write();
            }
            // Enable Interface... modification complete.
            Enabled = true;
            WinFormsUtil.Alert("Modified all Level ranges according to specification!", "Press the Dump Tables button to view the new Level ranges!");

            updatePanel(sender, e);
        }
        
        // Randomization
        private void B_Randomize_Click(object sender, EventArgs e)
        {
            if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Randomize all? Cannot undo.", "Double check Randomization settings at the bottom left.") != DialogResult.Yes) return;

            Enabled = false;
            int slotStart;
            int slotStop;
            bool copy = false;

            switch (CB_SlotRand.SelectedIndex)
            {
                default: // All
                    slotStart = 0;
                    slotStop = -1;
                    break;
                case 1: // Regular Only
                    slotStart = 0;
                    slotStop = 1;
                    break;
                case 2: // SOS Only
                    slotStart = 1;
                    slotStop = -1;
                    break;
                case 3: // Regular Only, Copy to SOS
                    slotStart = 0;
                    slotStop = 1;
                    copy = true;
                    break;
            }

            var rnd = new SpeciesRandomizer(Main.Config)
            {
                G1 = CHK_G1.Checked,
                G2 = CHK_G2.Checked,
                G3 = CHK_G3.Checked,
                G4 = CHK_G4.Checked,
                G5 = CHK_G5.Checked,
                G6 = CHK_G6.Checked,
                G7 = CHK_G7.Checked,

                E = CHK_E.Checked,
                L = CHK_L.Checked,
                rBST = CHK_BST.Checked,
            };
            rnd.Initialize();

            foreach (var Map in Areas)
            {
                foreach (var Table in Map.Tables)
                {
                    if (CHK_Level.Checked)
                    {
                        Table.MinLevel = Randomizer.getModifiedLevel(Table.MinLevel, NUD_LevelAmp.Value);
                        Table.MaxLevel = Randomizer.getModifiedLevel(Table.MaxLevel, NUD_LevelAmp.Value);
                    }

                    int end = slotStop < 0 ? Table.Encounters.Length : slotStop;
                    for (int s = slotStart; s < end; s++)
                    {
                        var EncounterSet = Table.Encounters[s];
                        foreach (var enc in EncounterSet.Where(enc => enc.Species != 0))
                        {
                            enc.Species = (uint)rnd.GetRandomSpecies((int)enc.Species);
                            enc.Forme = GetRandomForme((int) enc.Species);
                        }
                    }

                    if (copy) // copy row 0 to rest
                    {
                        var table = Table.Encounters;
                        var s0 = table[0];
                        for (int r = 1; r < table.Length; r++)
                        {
                            var slots = table[r];
                            for (int s = 0; s < slots.Length; s++)
                            {
                                slots[s].Species = s0[s].Species;
                                slots[s].Forme = s0[s].Forme;
                            }
                        }
                    }

                    Table.Write();
                }
                encdata[Map.FileNumber] = getMapData(Map.Tables);
            }
            updatePanel(sender, e);
            Enabled = true;
            WinFormsUtil.Alert("Randomized all Wild Encounters according to specification!", "Press the Dump Tables button to view the new Wild Encounter information!");
        }

        private uint GetRandomForme(int species)
        {
            if (Main.SpeciesStat[species].FormeCount <= 1)
                return 0;
            if (!Legal.Mega_ORAS.Contains((ushort) species) || CHK_MegaForm.Checked)
                return (uint) (Util.rnd32()%Main.SpeciesStat[species].FormeCount); // Slot-Random
            return 0;
        }

        private void CopySOS_Click(object sender, EventArgs e)
        {
            if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Copy initial species to SOS slots?", "Cannot undo.") != DialogResult.Yes) return;

            // first table is copied to all other tables except weather (last)
            for (int i = 1; i < nup_spec.Length - 1; i++)
            {
                for (int s = 0; s < nup_spec[i].Length; s++) // slot copy
                {
                    nup_spec[i][s].Value = nup_spec[0][s].Value;
                    cb_spec[i][s].SelectedIndex = cb_spec[0][s].SelectedIndex;
                }
            }
            WinFormsUtil.Alert("All initial species copied to SOS slots!");
        }
    }
}
