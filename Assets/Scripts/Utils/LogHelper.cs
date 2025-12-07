using UnityEngine;

public static class LogHelper
{
    // 必須参照の null チェック。
    // null の場合は LogError を出し false を返す。
    public static bool AssertNotNull(Object obj, string name, Object context)
    {
        if (obj != null) return true;

        // context を渡しておくと、ログから該当オブジェクトを選択できる
        Debug.LogError($"[{context.GetType().Name}] 必須参照 '{name}' が設定されていません。", context);
        return false;
    }
}
