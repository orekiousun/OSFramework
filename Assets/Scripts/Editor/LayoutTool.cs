using System;
using UnityEngine;

/// <summary>
/// Editor布局工具类
/// </summary>
public static class LayoutTool
{
    /// <summary>
    /// 水平排布元素
    /// </summary>
    /// <param name="action">在此action内生成元素</param>
    /// <param name="style">风格</param>
    /// <param name="options">额外选项（可以控制大小等）</param>
    public static void Horizontal(Action action, GUIStyle style, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(style, options);
        action?.Invoke();
        GUILayout.EndHorizontal();
    }
    
    /// <summary>
    /// 水平排布元素
    /// </summary>
    /// <param name="action"></param>
    /// <param name="options"></param>
    public static void Horizontal(Action action, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(options);
        action?.Invoke();
        GUILayout.EndHorizontal();
    }
    
    /// <summary>
    /// 垂直排布元素
    /// </summary>
    /// <param name="action"></param>
    /// <param name="style"></param>
    /// <param name="options"></param>
    public static void Vertical(Action action, GUIStyle style, params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(style, options);
        action?.Invoke();
        GUILayout.EndVertical();
    }
    
    /// <summary>
    /// 垂直排布元素
    /// </summary>
    /// <param name="action"></param>
    /// <param name="options"></param>
    public static void Vertical(Action action, params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(options);
        action?.Invoke();
        GUILayout.EndVertical();
    }

    /// <summary>
    /// 生成滚动条
    /// </summary>
    /// <param name="scrollPos"></param>
    /// <param name="action"></param>
    /// <param name="style"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static Vector2 ScrollView(Vector2 scrollPos, Action action, GUIStyle style, params GUILayoutOption[] options)
    {
        Vector2 ret = GUILayout.BeginScrollView(scrollPos, style, options);
        action?.Invoke();
        GUILayout.EndScrollView();
        return ret;
    }

    /// <summary>
    /// 生成滚动条
    /// </summary>
    /// <param name="scrollPos"></param>
    /// <param name="action"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static Vector2 ScrollView(Vector2 scrollPos, Action action, params GUILayoutOption[] options)
    {
        Vector2 ret = GUILayout.BeginScrollView(scrollPos, options);
        action?.Invoke();
        GUILayout.EndScrollView();
        return ret;
    }
    
    /// <summary>
    /// 生成滚动条
    /// </summary>
    /// <param name="scrollPos"></param>
    /// <param name="action"></param>
    /// <param name="style"></param>
    /// <param name="options"></param>
    public static void ScrollView(ref Vector2 scrollPos, Action action, GUIStyle style, params GUILayoutOption[] options)
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos, style, options);
        action?.Invoke();
        GUILayout.EndScrollView();
    }
    
    /// <summary>
    /// 生成滚动条
    /// </summary>
    /// <param name="scrollPos"></param>
    /// <param name="action"></param>
    /// <param name="options"></param>
    public static void ScrollView(ref Vector2 scrollPos, Action action, params GUILayoutOption[] options)
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos, options);
        action?.Invoke();
        GUILayout.EndScrollView();
    }
    
    /// <summary>
    /// 生成区域
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="action"></param>
    /// <param name="style"></param>
    public static void Area(Rect rect, Action action, GUIStyle style)
    {
        GUILayout.BeginArea(rect, style);
        action?.Invoke();
        GUILayout.EndArea();
    }

    /// <summary>
    /// 生成区域
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="action"></param>
    public static void Area(Rect rect, Action action)
    {
        GUILayout.BeginArea(rect);
        action?.Invoke();
        GUILayout.EndArea();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="action"></param>
    /// <param name="width"></param>
    public static void SurroundSpace(Action action, float width = 6f)
    {
        GUILayout.Space(width);
        action?.Invoke();
        GUILayout.Space(width);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <param name="action"></param>
    public static void SurroundColor(Color color, Action action)
    {
        Color oriColor = GUI.color;
        GUI.color = color;
        action?.Invoke();
        GUI.color = oriColor;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="con"></param>
    /// <param name="color"></param>
    /// <param name="action"></param>
    public static void SurroundColor(bool con, Color color, Action action)
    {
        if (con)
        {
            SurroundColor(color, action);
        }
        else
        {
            action?.Invoke();
        }
    }
}