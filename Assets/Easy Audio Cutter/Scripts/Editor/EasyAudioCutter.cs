namespace EasyAudioCutter
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;

    public class EasyAudioCutter : EditorWindow
    {
        private AudioClip _originalClip;
        private float _startTime = 0f;
        private float _endTime = 2f;
        private string _outputPath = "Assets/TrimmedAudio.wav";
        private AudioSource _previewSource;
        private float _previewProgress = 0f;
        private bool _isPreviewPlaying = false;
        private double _previewStartTime;
        private Vector2 _waveformScrollPosition;
        private Rect _waveformRect;
        private bool _isDragging = false;
        private Texture2D _waveformTexture;
        private OutputFormat _outputFormat = OutputFormat.WAV;
        private float _zoomLevel = 1f;
        private List<(float start, float end)> _undoStack = new List<(float, float)>();
        private List<(float start, float end)> _redoStack = new List<(float, float)>();
        private bool _loopPreview = false;
        private bool _isTrimmingInProgress = false; // Trim işlemi sırasında kilit için
        private float _trimProgress = 0f; // Kaydetme yüzdesi
        private double _trimStartTime; // Kaydetme başlangıç zamanı

        private enum OutputFormat
        {
            WAV,
            MP3
        }

        private static readonly Color BackgroundColor = new Color(0.15f, 0.15f, 0.15f);
        private static readonly Color SectionColor = new Color(0.25f, 0.25f, 0.25f);
        private static readonly Color AccentColor = new Color(0.1f, 0.6f, 0.9f);
        private static readonly Color TextColor = Color.white;
        private static readonly Color SelectionColor = new Color(0.1f, 0.6f, 0.9f, 0.4f);
        private static readonly Color HoverColor = new Color(0.15f, 0.7f, 1f);
        private static readonly Color PlaybackLineColor = Color.red;
        private static readonly Color TimeMarkerColor = new Color(0f, 0f, 0f, 1f);

        private Texture2D _playIcon;
        private Texture2D _stopIcon;
        private Texture2D _trimIcon;
        private Texture2D _undoIcon;
        private Texture2D _redoIcon;
        private Texture2D _helpIcon;

        [MenuItem("Assets/Easy Audio Cutter", false, 20)]
        public static void OpenFromContext()
        {
            AudioClip selectedClip = Selection.activeObject as AudioClip;
            if (selectedClip == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select an audio file!", "OK");
                return;
            }

            EasyAudioCutter window = GetWindow<EasyAudioCutter>("Easy Audio Cutter");
            window.Initialize(selectedClip);
            window.Show();
        }

        [MenuItem("Assets/Easy Audio Cutter", true)]
        public static bool ValidateOpenFromContext()
        {
            return Selection.activeObject is AudioClip;
        }

        public void Initialize(AudioClip clip)
        {
            _originalClip = clip;
            _outputPath = AssetDatabase.GetAssetPath(clip).Replace(".wav", "_trimmed.wav");
            _endTime = clip.length;
            GenerateWaveformTexture();
            _undoStack.Clear();
            _undoStack.Add((0f, clip.length));
            InitializeIcons();
        }

        private void InitializeIcons()
        {
            _playIcon = EditorGUIUtility.IconContent("d_PlayButton").image as Texture2D;
            _stopIcon = EditorGUIUtility.IconContent("d_PauseButton").image as Texture2D;
            _trimIcon = EditorGUIUtility.IconContent("d_editicon.sml").image as Texture2D;
            _undoIcon = EditorGUIUtility.IconContent("d_RotateTool").image as Texture2D;
            _redoIcon = EditorGUIUtility.IconContent("d_RotateTool On").image as Texture2D;
            _helpIcon = EditorGUIUtility.IconContent("d_UnityEditor.ConsoleWindow").image as Texture2D;
        }

        void OnGUI()
        {
            GUI.backgroundColor = BackgroundColor;

            GUIStyle originalHorizontalScrollbar = GUI.skin.horizontalScrollbar;
            GUIStyle originalHorizontalScrollbarThumb = GUI.skin.horizontalScrollbarThumb;
            GUIStyle originalVerticalScrollbar = GUI.skin.verticalScrollbar;
            GUIStyle originalVerticalScrollbarThumb = GUI.skin.verticalScrollbarThumb;

            GUI.skin.horizontalScrollbar = new GUIStyle(GUI.skin.horizontalScrollbar)
            {
                normal = { background = MakeTexture(2, 2, SectionColor) },
                hover = { background = MakeTexture(2, 2, SectionColor * 1.2f) }
            };
            GUI.skin.horizontalScrollbarThumb = new GUIStyle(GUI.skin.horizontalScrollbarThumb)
            {
                normal = { background = MakeTexture(2, 2, AccentColor) },
                hover = { background = MakeTexture(2, 2, AccentColor * 1.2f) }
            };
            GUI.skin.verticalScrollbar = new GUIStyle(GUI.skin.verticalScrollbar)
            {
                normal = { background = MakeTexture(2, 2, SectionColor) },
                hover = { background = MakeTexture(2, 2, SectionColor * 1.2f) }
            };
            GUI.skin.verticalScrollbarThumb = new GUIStyle(GUI.skin.verticalScrollbarThumb)
            {
                normal = { background = MakeTexture(2, 2, AccentColor) },
                hover = { background = MakeTexture(2, 2, AccentColor * 1.2f) }
            };

            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = TextColor },
                fontStyle = FontStyle.Bold
            };

            GUIStyle sectionStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 10, 10),
                normal = { background = MakeTexture(2, 2, SectionColor) },
                border = new RectOffset(4, 4, 4, 4)
            };

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(10, 10, 5, 5),
                normal = { textColor = TextColor, background = MakeTexture(2, 2, AccentColor) },
                hover = { textColor = TextColor, background = MakeTexture(2, 2, HoverColor) },
                active = { background = MakeTexture(2, 2, AccentColor * 0.8f) },
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUIStyle rangeStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = TextColor },
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };

            GUIStyle timeLabelStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = TextColor },
                alignment = TextAnchor.UpperCenter,
                fontSize = 10
            };

            EditorGUILayout.Space(15);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(_undoStack.Count <= 1 || _isTrimmingInProgress);
            DrawButtonWithHover(new GUIContent("Undo", _undoIcon), buttonStyle, UndoSelection);
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(_redoStack.Count == 0 || _isTrimmingInProgress);
            DrawButtonWithHover(new GUIContent("Redo", _redoIcon), buttonStyle, RedoSelection);
            EditorGUI.EndDisabledGroup();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Easy Audio Cutter", headerStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Help", _helpIcon), buttonStyle, GUILayout.Width(70)))
            {
                ShowHelp();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(15);

            EditorGUI.BeginDisabledGroup(true);
            _originalClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", _originalClip, typeof(AudioClip), false, GUILayout.Height(20));
            EditorGUI.EndDisabledGroup();

            if (_originalClip != null)
            {
                float clipLength = _originalClip.length;

                EditorGUI.BeginDisabledGroup(_isTrimmingInProgress);
                EditorGUILayout.BeginVertical(sectionStyle);
                EditorGUILayout.LabelField("Waveform (Scroll to zoom)", EditorStyles.miniBoldLabel);
                DrawWaveform(clipLength);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                GUIContent previewContent = _isPreviewPlaying ?
                    new GUIContent("Stop Preview", _stopIcon) :
                    new GUIContent("Preview", _playIcon);
                if (DrawButtonWithHover(previewContent, buttonStyle, null))
                {
                    if (_isPreviewPlaying)
                    {
                        StopPreview();
                    }
                    else
                    {
                        PlayPreview();
                    }
                }

                if (GUILayout.Button(_loopPreview ? "Loop: On" : "Loop: Off", buttonStyle, GUILayout.Width(90)))
                {
                    _loopPreview = !_loopPreview;
                    if (_previewSource != null && _previewSource.isPlaying)
                    {
                        _previewSource.loop = _loopPreview;
                    }
                    Debug.Log("Loop Preview changed to: " + _loopPreview);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space(10);

                EditorGUI.BeginDisabledGroup(_isTrimmingInProgress);
                EditorGUILayout.BeginVertical(sectionStyle);
                EditorGUILayout.LabelField("Trim Settings", EditorStyles.miniBoldLabel);
                EditorGUI.BeginChangeCheck();
                DrawCustomMinMaxSlider(ref _startTime, ref _endTime, 0f, clipLength);
                EditorGUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Start:", GUILayout.Width(40));
                _startTime = EditorGUILayout.FloatField(_startTime, GUILayout.Width(100));
                _startTime = Mathf.Clamp(_startTime, 0f, _endTime);
                EditorGUILayout.LabelField("End:", GUILayout.Width(40));
                _endTime = EditorGUILayout.FloatField(_endTime, GUILayout.Width(100));
                _endTime = Mathf.Clamp(_endTime, _startTime, clipLength);
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    SaveSelectionForUndo();
                }
                EditorGUILayout.EndVertical();
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space(10);

                EditorGUILayout.BeginVertical(sectionStyle);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Selected Range: {_startTime:F2}s - {_endTime:F2}s", rangeStyle, GUILayout.ExpandWidth(false));
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField($"Duration: {(_endTime - _startTime):F2}s", rangeStyle, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space(10);

                EditorGUI.BeginDisabledGroup(_isTrimmingInProgress);
                EditorGUILayout.BeginVertical(sectionStyle);
                EditorGUILayout.LabelField("Output Settings", EditorStyles.miniBoldLabel);
                EditorGUILayout.BeginHorizontal();
                _outputPath = EditorGUILayout.TextField("Output Path", _outputPath);
                _outputFormat = (OutputFormat)EditorGUILayout.EnumPopup(_outputFormat, GUILayout.Width(70));
                EditorGUILayout.EndHorizontal();

                if (_outputFormat == OutputFormat.WAV && !_outputPath.EndsWith(".wav"))
                {
                    _outputPath = _outputPath.Replace(".mp3", ".wav");
                }
                else if (_outputFormat == OutputFormat.MP3 && !_outputPath.EndsWith(".mp3"))
                {
                    _outputPath = _outputPath.Replace(".wav", ".mp3");
                }
                EditorGUILayout.EndVertical();
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space(15);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (!_isTrimmingInProgress)
                {
                    if (DrawButtonWithHover(new GUIContent("Trim Audio", _trimIcon), buttonStyle, null))
                    {
                        _isTrimmingInProgress = true;
                        _trimProgress = 0f;
                        _trimStartTime = EditorApplication.timeSinceStartup;
                        TrimAudio(); // Kesim işlemi başlıyor
                    }
                }
                else
                {
                    Rect progressRect = EditorGUILayout.GetControlRect(GUILayout.Height(20));
                    EditorGUI.DrawRect(progressRect, SectionColor);
                    Rect fillRect = new Rect(progressRect.x, progressRect.y, progressRect.width * _trimProgress, progressRect.height);
                    EditorGUI.DrawRect(fillRect, AccentColor);
                    EditorGUI.LabelField(progressRect, $"Saving: {(_trimProgress * 100):F0}%", EditorStyles.miniLabel);
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(10);

                EditorGUI.BeginDisabledGroup(_isTrimmingInProgress);
                if (_isPreviewPlaying && _previewSource != null && _previewSource.isPlaying)
                {
                    _previewSource.loop = _loopPreview;
                    float previewDuration = _endTime - _startTime;
                    float elapsedTime = (float)(EditorApplication.timeSinceStartup - _previewStartTime);
                    _previewProgress = Mathf.Clamp01(elapsedTime / previewDuration);
                    Rect progressRect = EditorGUILayout.GetControlRect(GUILayout.Height(10));
                    EditorGUI.DrawRect(progressRect, SectionColor);
                    Rect fillRect = new Rect(progressRect.x, progressRect.y, progressRect.width * _previewProgress, progressRect.height);
                    EditorGUI.DrawRect(fillRect, AccentColor);
                    EditorGUI.LabelField(progressRect, "Preview Progress", EditorStyles.miniLabel);
                }
                else if (_isPreviewPlaying)
                {
                    _isPreviewPlaying = false;
                    _previewProgress = 0f;
                }
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.LabelField("Please select an audio file.", EditorStyles.centeredGreyMiniLabel);
            }

            EditorGUILayout.Space(15);

            GUI.skin.horizontalScrollbar = originalHorizontalScrollbar;
            GUI.skin.horizontalScrollbarThumb = originalHorizontalScrollbarThumb;
            GUI.skin.verticalScrollbar = originalVerticalScrollbar;
            GUI.skin.verticalScrollbarThumb = originalVerticalScrollbarThumb;
        }

        private void ShowHelp()
        {
            EditorUtility.DisplayDialog("Help - Easy Audio Cutter",
                "Welcome to Easy Audio Cutter!\n\n" +
                "1. **Waveform**: View the audio waveform. Scroll with the mouse wheel to zoom in/out, or left-click and drag to select a range.\n" +
                "2. **Trim Settings**: Adjust the start and end times using the slider or input fields.\n" +
                "3. **Preview/Stop**: Click 'Preview' to play the selected range; it becomes 'Stop Preview' during playback. Click again to stop. Enable 'Loop: On' to repeat continuously.\n" +
                "4. **Trim Audio**: Click 'Trim Audio' to start saving the trimmed audio. A progress bar will show the saving percentage. Other actions are locked until complete.\n" +
                "5. **Undo/Redo**: Revert or reapply your last changes to the trim selection.\n" +
                "6. **Output Settings**: Set the output path and format. Currently, only WAV is supported; for MP3, use an external tool like Audacity after saving.\n\n" +
                "**Tips:**\n" +
                "- Right-click an audio file in the Project window and select 'Easy Audio Cutter' to start.\n" +
                "- Change the 'Output Path' to save the trimmed file to a custom location.\n" +
                "- For very long audio files, waveform generation may take a few seconds.\n\n" +
                "**Troubleshooting:**\n" +
                "- If you see 'Invalid trim range', ensure start and end times are within the audio duration.\n" +
                "- If the tool doesn’t respond, check that an audio file is selected.",
                "OK");
        }

        private bool DrawButtonWithHover(GUIContent content, GUIStyle style, Action onClick)
        {
            Rect buttonRect = GUILayoutUtility.GetRect(content, style);
            Event e = Event.current;

            bool clicked = false;
            if (buttonRect.Contains(e.mousePosition))
            {
                EditorGUI.DrawRect(buttonRect, new Color(1f, 1f, 1f, 0.05f));
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    onClick?.Invoke();
                    clicked = true;
                    e.Use();
                }
            }

            GUI.Button(buttonRect, content, style);
            return clicked;
        }

        private void DrawCustomMinMaxSlider(ref float minValue, ref float maxValue, float minLimit, float maxLimit)
        {
            Rect controlRect = EditorGUILayout.GetControlRect(GUILayout.Height(20));
            Event e = Event.current;

            EditorGUI.DrawRect(controlRect, new Color(0.3f, 0.3f, 0.3f));
            float minPos = controlRect.x + (minValue / maxLimit) * controlRect.width;
            float maxPos = controlRect.x + (maxValue / maxLimit) * controlRect.width;
            EditorGUI.DrawRect(new Rect(minPos, controlRect.y, maxPos - minPos, controlRect.height), SelectionColor);

            Rect minThumbRect = new Rect(minPos - 6, controlRect.y, 12, controlRect.height);
            Rect maxThumbRect = new Rect(maxPos - 6, controlRect.y, 12, controlRect.height);
            EditorGUI.DrawRect(minThumbRect, AccentColor);
            EditorGUI.DrawRect(maxThumbRect, AccentColor);

            if (controlRect.Contains(e.mousePosition))
            {
                if (e.type == EventType.MouseDown || (e.type == EventType.MouseDrag && e.button == 0))
                {
                    float mousePos = e.mousePosition.x;
                    float minDistance = Mathf.Abs(mousePos - minPos);
                    float maxDistance = Mathf.Abs(mousePos - maxPos);

                    if (minDistance < maxDistance)
                    {
                        minValue = Mathf.Clamp((mousePos - controlRect.x) / controlRect.width * maxLimit, minLimit, maxValue);
                    }
                    else
                    {
                        maxValue = Mathf.Clamp((mousePos - controlRect.x) / controlRect.width * maxLimit, minValue, maxLimit);
                    }
                    Repaint();
                }
            }
        }

        private void GenerateWaveformTexture()
        {
            if (_originalClip == null) return;

            int baseWidth = Mathf.Min(2000, _originalClip.samples / 100);
            int height = 100;
            float[] samples = new float[_originalClip.samples * _originalClip.channels];
            _originalClip.GetData(samples, 0);

            _waveformTexture = new Texture2D(baseWidth, height, TextureFormat.RGBA32, false);
            Color[] colors = new Color[baseWidth * height];

            for (int y = 0; y < height; y++)
            {
                float t = (float)y / height;
                Color gradientColor = Color.Lerp(SectionColor, new Color(0.5f, 0.5f, 0.5f), t);
                for (int x = 0; x < baseWidth; x++)
                {
                    colors[x + y * baseWidth] = gradientColor;
                }
            }

            int samplesPerPixel = Mathf.Max(1, _originalClip.samples / baseWidth);
            for (int i = 0; i < baseWidth; i++)
            {
                float min = 1f;
                float max = -1f;
                int startSample = i * samplesPerPixel;
                int endSample = Mathf.Min(startSample + samplesPerPixel, samples.Length);

                for (int j = startSample; j < endSample; j++)
                {
                    float sample = samples[j];
                    min = Mathf.Min(min, sample);
                    max = Mathf.Max(max, sample);
                }

                int yCenter = height / 2;
                int yMin = Mathf.Clamp(yCenter - (int)(min * yCenter), 0, height - 1);
                int yMax = Mathf.Clamp(yCenter - (int)(max * yCenter), 0, height - 1);

                for (int y = Mathf.Min(yMin, yMax); y <= Mathf.Max(yMin, yMax); y++)
                {
                    colors[i + y * baseWidth] = Color.Lerp(TextColor, AccentColor, (float)y / height);
                }
            }

            _waveformTexture.SetPixels(colors);
            _waveformTexture.Apply();
        }

        private void DrawWaveform(float clipLength)
        {
            _waveformScrollPosition = EditorGUILayout.BeginScrollView(_waveformScrollPosition, GUILayout.Height(100));
            float zoomedWidth = _waveformTexture != null ? _waveformTexture.width * _zoomLevel : 500 * _zoomLevel;
            _waveformRect = GUILayoutUtility.GetRect(zoomedWidth, 100);

            if (_waveformTexture != null)
            {
                GUI.DrawTexture(_waveformRect, _waveformTexture, ScaleMode.StretchToFill);

                float startX = _waveformRect.x + (_startTime / clipLength) * _waveformRect.width;
                float endX = _waveformRect.x + (_endTime / clipLength) * _waveformRect.width;
                EditorGUI.DrawRect(new Rect(startX, _waveformRect.y, endX - startX, _waveformRect.height), SelectionColor);

                if (_isPreviewPlaying && _previewSource != null && _previewSource.isPlaying)
                {
                    float previewDuration = _endTime - _startTime;
                    float elapsedTime = (float)(EditorApplication.timeSinceStartup - _previewStartTime);
                    float playbackPosition;

                    if (_loopPreview)
                    {
                        playbackPosition = _startTime + (elapsedTime % previewDuration);
                    }
                    else
                    {
                        playbackPosition = _startTime + Mathf.Min(elapsedTime, previewDuration);
                    }

                    float playbackX = _waveformRect.x + (playbackPosition / clipLength) * _waveformRect.width;
                    EditorGUI.DrawRect(new Rect(playbackX, _waveformRect.y, 2, _waveformRect.height), PlaybackLineColor);
                }

                Event e = Event.current;
                if (_waveformRect.Contains(e.mousePosition))
                {
                    if (e.type == EventType.MouseDown && e.button == 0)
                    {
                        _isDragging = true;
                        _startTime = Mathf.Clamp((e.mousePosition.x - _waveformRect.x) / _waveformRect.width * clipLength, 0, clipLength);
                        _endTime = _startTime;
                        SaveSelectionForUndo();
                        Repaint();
                    }
                    if (e.type == EventType.MouseDrag && _isDragging)
                    {
                        _endTime = Mathf.Clamp((e.mousePosition.x - _waveformRect.x) / _waveformRect.width * clipLength, 0, clipLength);
                        if (_endTime < _startTime)
                        {
                            float temp = _startTime;
                            _startTime = _endTime;
                            _endTime = temp;
                        }
                        Repaint();
                    }
                    if (e.type == EventType.MouseUp && _isDragging)
                    {
                        _isDragging = false;
                        SaveSelectionForUndo();
                        Repaint();
                    }
                    if (e.type == EventType.ScrollWheel)
                    {
                        _zoomLevel = Mathf.Clamp(_zoomLevel + (e.delta.y > 0 ? -0.2f : 0.2f), 1f, 10f);
                        Repaint();
                    }
                }
            }
            else
            {
                EditorGUI.LabelField(_waveformRect, "Waveform could not be generated!", EditorStyles.centeredGreyMiniLabel);
            }
            EditorGUILayout.EndScrollView();

            if (_waveformTexture != null)
            {
                Rect labelArea = GUILayoutUtility.GetRect(zoomedWidth, 20);
                float visibleWidth = position.width - 20;
                float startXVisible = _waveformScrollPosition.x;
                float endXVisible = startXVisible + visibleWidth;
                int maxTime = Mathf.CeilToInt(clipLength);

                GUIStyle labelStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.UpperCenter,
                    normal = { textColor = TextColor },
                    fontSize = 10
                };

                for (int time = 0; time <= maxTime; time++)
                {
                    float xPos = (time / clipLength) * zoomedWidth;
                    if (xPos >= startXVisible && xPos <= endXVisible)
                    {
                        float adjustedXPos = labelArea.x + xPos - _waveformScrollPosition.x;
                        Rect labelRect = new Rect(adjustedXPos - 10, labelArea.y, 20, 20);
                        EditorGUI.LabelField(labelRect, $"{time}", labelStyle);
                    }
                }
            }
        }

        private void SaveSelectionForUndo()
        {
            _undoStack.Add((_startTime, _endTime));
            _redoStack.Clear();
        }

        private void UndoSelection()
        {
            if (_undoStack.Count > 1)
            {
                var current = _undoStack[_undoStack.Count - 1];
                _undoStack.RemoveAt(_undoStack.Count - 1);
                _redoStack.Add(current);
                var previous = _undoStack[_undoStack.Count - 1];
                _startTime = previous.start;
                _endTime = previous.end;
                Repaint();
            }
        }

        private void RedoSelection()
        {
            if (_redoStack.Count > 0)
            {
                var next = _redoStack[_redoStack.Count - 1];
                _redoStack.RemoveAt(_redoStack.Count - 1);
                _undoStack.Add(next);
                _startTime = next.start;
                _endTime = next.end;
                Repaint();
            }
        }

        private void PlayPreview()
        {
            int sampleRate = _originalClip.frequency;
            int totalSamples = _originalClip.samples;
            int channels = _originalClip.channels;

            int startSample = Mathf.FloorToInt(_startTime * sampleRate);
            int endSample = Mathf.FloorToInt(_endTime * sampleRate);

            if (startSample < 0 || endSample > totalSamples || startSample >= endSample)
            {
                EditorUtility.DisplayDialog("Error", "Invalid preview range!", "OK");
                return;
            }

            int trimmedLength = endSample - startSample;

            float[] samples = new float[totalSamples * channels];
            _originalClip.GetData(samples, 0);

            float[] trimmedSamples = new float[trimmedLength * channels];
            Array.Copy(samples, startSample * channels, trimmedSamples, 0, trimmedLength * channels);

            AudioClip previewClip = AudioClip.Create("Preview", trimmedLength, channels, sampleRate, false);
            previewClip.SetData(trimmedSamples, 0);

            if (_previewSource == null)
            {
                GameObject tempObj = new GameObject("TempAudioSource");
                _previewSource = tempObj.AddComponent<AudioSource>();
                tempObj.hideFlags = HideFlags.HideAndDontSave;
            }

            _previewSource.clip = previewClip;
            _previewSource.loop = _loopPreview;
            _previewSource.Play();

            _isPreviewPlaying = true;
            _previewStartTime = EditorApplication.timeSinceStartup;
        }

        private void StopPreview()
        {
            if (_previewSource != null && _previewSource.isPlaying)
            {
                _previewSource.Stop();
                _isPreviewPlaying = false;
                _previewProgress = 0f;
            }
        }

        private void TrimAudio()
        {
            int sampleRate = _originalClip.frequency;
            int totalSamples = _originalClip.samples;
            int channels = _originalClip.channels;

            int startSample = Mathf.FloorToInt(_startTime * sampleRate);
            int endSample = Mathf.FloorToInt(_endTime * sampleRate);

            if (startSample < 0 || endSample > totalSamples || startSample >= endSample)
            {
                EditorUtility.DisplayDialog("Error", "Invalid trim range!", "OK");
                _isTrimmingInProgress = false;
                return;
            }

            int trimmedLength = endSample - startSample;

            float[] samples = new float[totalSamples * channels];
            _originalClip.GetData(samples, 0);

            float[] trimmedSamples = new float[trimmedLength * channels];
            Array.Copy(samples, startSample * channels, trimmedSamples, 0, trimmedLength * channels);

            AudioClip trimmedClip = AudioClip.Create("TrimmedAudio", trimmedLength, channels, sampleRate, false);
            trimmedClip.SetData(trimmedSamples, 0);

            SaveTrimmedAudio(trimmedClip);
        }

        private void SaveTrimmedAudio(AudioClip clip)
        {
            WavSaver.Save(_outputPath, clip);
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(_outputPath);

            AudioClip importedClip = AssetDatabase.LoadAssetAtPath<AudioClip>(_outputPath);
            if (importedClip != null)
            {
                Debug.Log("Audio file successfully trimmed and saved: " + _outputPath);
                if (_outputFormat == OutputFormat.MP3)
                {
                    EditorUtility.DisplayDialog("Note", "The file is saved as WAV. To convert to MP3, use an external tool like Audacity.", "OK");
                }
            }
            else
            {
                Debug.LogError("File saved but could not be loaded as AudioClip.");
            }
        }

        void OnDestroy()
        {
            if (_previewSource != null)
            {
                DestroyImmediate(_previewSource.gameObject);
                _previewSource = null;
            }
            if (_waveformTexture != null)
            {
                DestroyImmediate(_waveformTexture);
                _waveformTexture = null;
            }
        }

        void Update()
        {
            if (_isPreviewPlaying)
            {
                Repaint();
            }
            if (_isTrimmingInProgress)
            {
                // Simüle edilen kaydetme süresi (örneğin 2 saniye)
                float trimDuration = 2f;
                float elapsedTime = (float)(EditorApplication.timeSinceStartup - _trimStartTime);
                _trimProgress = Mathf.Clamp01(elapsedTime / trimDuration);

                if (_trimProgress >= 1f)
                {
                    _isTrimmingInProgress = false;
                    _trimProgress = 0f;
                }
                Repaint();
            }
        }

        private Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }
    }

    public static class WavSaver
    {
        public static void Save(string filepath, AudioClip clip)
        {
            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            using (var memoryStream = new System.IO.MemoryStream())
            using (var writer = new System.IO.BinaryWriter(memoryStream))
            {
                int channels = clip.channels;
                int sampleRate = clip.frequency;
                int sampleCount = clip.samples;

                writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
                writer.Write(36 + samples.Length * 2);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
                writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(sampleRate);
                writer.Write(sampleRate * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
                writer.Write(samples.Length * 2);

                foreach (float sample in samples)
                {
                    short value = (short)(sample * 32767);
                    writer.Write(value);
                }

                byte[] wavBytes = memoryStream.ToArray();
                System.IO.File.WriteAllBytes(filepath, wavBytes);
            }
        }
    }
}