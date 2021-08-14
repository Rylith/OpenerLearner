﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using ImGuiScene;

namespace OpenerHelper
{
    unsafe class Drawer : IDisposable
    {
        OpenerHelper p;

        private Vector2 prevCursorPos = new Vector2();
        private TextureWrap TestImg;

        static Vector2 Vector2Scale = new Vector2(48f, 48f);
        private Dictionary<uint, TextureWrap> textures;

        public Drawer(OpenerHelper p)
        {
            this.p = p;
            p.pi.UiBuilder.OnBuildUi += Draw;
            textures = new Dictionary<uint, TextureWrap>();
            TestImg = p.pi.UiBuilder.LoadImage(Path.Combine(Path.GetDirectoryName(p.AssemblyLocation), "Test.png"));
        }

        public void Dispose()
        {
            p.pi.UiBuilder.OnBuildUi -= Draw;
            foreach (var t in textures.Values)
            {
                t.Dispose();
            }
        }

        private void Draw()
        {
            ImGui.SetNextWindowSize(new Vector2(200, 200), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("OpenerLearner", ImGuiWindowFlags.MenuBar))
            {
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("Choix"))
                    {
                        if (ImGui.MenuItem("Opener"))
                        {
                            // ...
                        }
                        if (ImGui.MenuItem("Rotation"))
                        {
                            // ...
                        }
                        ImGui.EndMenu();
                    }
                    ImGui.EndMenuBar();
                }
                //ImGui.Text("Some text inside window");
                for (int i = 0; i < p.currentSkills.Length; i++)
                {
                    prevCursorPos = ImGui.GetCursorPos();
                    ImGuiDrawSkill(p.currentSkills[i]);
                    if (i == p.currentSkill)
                    {
                        ImGui.SetCursorPos(prevCursorPos);
                        ImGui.Image(TestImg.ImGuiHandle, Vector2Scale, Vector2.Zero, Vector2.One, Vector4.One);

                    }
                    ImGui.SameLine();
                }
            }
            ImGui.End();
        }

        void ImGuiDrawSkill(uint id)
        {
            try
            {
                if (!textures.ContainsKey(id))
                {
                    textures[id] = p.pi.Data.GetImGuiTexture(p.pi.Data.GetIcon(p.ActionsDic[id].Icon).FilePath.Path.Replace(".tex", "_hr1.tex"));
                }
                //ImGui.SetCursorPos((ImGui.GetWindowSize() - new Vector2(textures[id].Height, textures[id].Width)) * 0.5f); //To center the image
                ImGui.Image(textures[id].ImGuiHandle, Vector2Scale, Vector2.Zero, Vector2.One, Vector4.One);
            }
            catch (Exception ex)
            {
                p.pi.Framework.Gui.Chat.Print("[Error] " + ex.Message + "\n" + ex.StackTrace);
            }
        }
        private string FindTexture()
        {
            var o = p.pi.Framework.Gui.GetUiObjectByName("_ActionBar01", 1);
            var masterWindow = (AtkUnitBase*)o;
            var skillCNode = (AtkComponentNode*)masterWindow->UldManager.NodeList[10];
            var skillCNode2 = (AtkComponentNode*)skillCNode->Component->UldManager.NodeList[0];
            var skillCNode3 = (AtkComponentNode*)skillCNode2->Component->UldManager.NodeList[2];
            var skillImage = (AtkImageNode*)skillCNode3->Component->UldManager.NodeList[0];

            var textureInfo = skillImage->PartsList->Parts[skillImage->PartId].UldAsset;
            if (textureInfo->AtkTexture.TextureType == TextureType.Resource)
            {
                var texturePath = Marshal.PtrToStringAnsi((IntPtr)textureInfo->AtkTexture.Resource->TexFileResourceHandle->ResourceHandle.FileName);
                //textures[texturePath] = pi.Data.GetImGuiTexture(texturePath);
                return texturePath;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}