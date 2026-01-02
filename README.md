## Relic Guardian

https://github.com/user-attachments/assets/add79d30-f475-49c1-9134-6e2e63574aa3

<img width="967" height="540" alt="image" src="https://github.com/user-attachments/assets/fdf2d275-ec8d-4716-b8c2-8c3cc04dad31" />

<img width="1052" height="589" alt="image" src="https://github.com/user-attachments/assets/e361b8f9-0a8d-4728-ba0e-56e708b185e6" />

### ゲームについて
#### URL
https://unityroom.com/games/relic_guardian

#### 操作方法
```
【ゲーム中】
[A][S][D]: 移動,壁滑り降り
[J]: 攻撃
[Space]: ジャンプ (2段まで)
[左Shift] [K] [L]: 対応したスキルを使う
[ESC]: ステータス画面を開く

【メニュー・タイトル・ステータス画面】
[J] / [Enter] : 決定
[ESC] : 戻る
```

#### ゲームの説明
キーボードで操作する 2D 横スクロールアクションです。

フィールドに存在するクリスタルを一定時間護り抜くゲームです。

護りながら、最後に現れるボスを倒せばクリアになります。


### 開発環境等
* 製作時間：170時間ほど
* Unity Ver：6000.0.60f1
* ジャンル：2D アクション
* 作業範囲：ゲームデザイン / プログラム / UIなど


### 作成の目的
以下の理解を目的としました。
* StateMachineを使用した、Objectの状態遷移の流れ
* アセットとして用意したSpriteの割当て方法
* Animatorを使用したキャラクターアニメーションの流れ
* Tilemapを使用したステージの作成手順
* パララックスな背景の仕組み
* Cinemachineを使用したカメラの移動、画面揺れ等の演出方法
* NewInputSetを使用した、Player操作とUI操作の切り離し方法
* 敵味方の共通化の仕組み
* キャラクタースキルの実装、アンロックから習得までの大まかな流れ
* 被弾時のVFX, SFXの実装方法


### 考慮した部分など
#### 状態遷移
* 待機、移動などそれぞれのStateを管理するためのStateMachineの用意
  * https://github.com/skonishi1125/2d_arena_action_unity/blob/efc5c28d28a8aa4f5afe1cba469a672192768962/Assets/Scripts/Etc/StateMachine.cs
* 全ての状態の親クラスEntityStateを用意し、そこから敵味方別にStateを管理するようにした
* https://github.com/skonishi1125/2d_arena_action_unity/blob/main/Assets/Scripts/Entity/EntityState.cs
  * https://github.com/skonishi1125/2d_arena_action_unity/blob/efc5c28d28a8aa4f5afe1cba469a672192768962/Assets/Scripts/Player/States/PlayerState.cs
    * https://github.com/skonishi1125/2d_arena_action_unity/blob/efc5c28d28a8aa4f5afe1cba469a672192768962/Assets/Scripts/Player/States/PlayerIdleState.cs
  * https://github.com/skonishi1125/2d_arena_action_unity/blob/main/Assets/Scripts/Enemy/EnemyState.cs
    * https://github.com/skonishi1125/2d_arena_action_unity/blob/main/Assets/Scripts/Enemy/EnemyIdleState.cs


#### Player, Enemy
* 敵味方共通の親クラス Entity を用意し、ヘルスや各種ステータスなどの仕組みを共通化
  * https://github.com/skonishi1125/2d_arena_action_unity/blob/main/Assets/Scripts/Entity/Entity.cs
    * https://github.com/skonishi1125/2d_arena_action_unity/blob/main/Assets/Scripts/Player/Player.cs
    * https://github.com/skonishi1125/2d_arena_action_unity/blob/main/Assets/Scripts/Enemy/Enemy.cs
* ヘルスや各種ステータスなど、項目をComponentとして分割して割り当てた
  * https://github.com/skonishi1125/2d_arena_action_unity/blob/efc5c28d28a8aa4f5afe1cba469a672192768962/Assets/Scripts/Entity/EntityHealth.cs
  * https://github.com/skonishi1125/2d_arena_action_unity/blob/efc5c28d28a8aa4f5afe1cba469a672192768962/Assets/Scripts/Entity/EntityStatus.cs
* Animatorの導入
  * BlendTree(ジャンプ/落下など）含む、State別アニメーションの用意
  * SpriteEditorでの切り出し、ピボットポイントの調整
  * Parametersを用いたStateの切り替え
* プレイヤースキル関連
  * ScriptableObjectとしてスキルデータを管理する形とした
    * https://github.com/skonishi1125/2d_arena_action_unity/blob/main/Assets/Scripts/Player/Skills/SkillDefinition.cs
  * パッシブスキル、アクティブスキルの用意
  * 物理か魔法、どちらかのビルドを選択できるような設計とした
  * スキルを使ったときのクールタイムを可視化
    * <img width="242" height="114" alt="image" src="https://github.com/user-attachments/assets/54f28e6e-b002-4d35-a19c-ad9a4da6f60e" />
  * スキルのアンロックからレベルアップまでの流れの用意
* 敵AIの実装
  * プレイヤーを狙うのか、クリスタルを狙うのか
  * Raycastを用いたプレイヤー、壁、地面などの感知処理
  * ゲームオーバーとなった場合のState中断処理
* Gizmoを使った地面や壁の検知、攻撃範囲の可視化
  * <img width="1238" height="661" alt="image" src="https://github.com/user-attachments/assets/beae97fd-d1ee-4b82-818a-c7d022d76d8d" />


#### フィールド
* Cinemachineを使用したカメラの移動、画面揺れ等の演出
* アセットをTilemapとして、地面と背景の一部に使用
* ペイントアプリ(Clip Studio Paint)を使用したアセットの編集※許可されたもののみ。
  * Tilemapとして切り出すため、素材の並び替え対応
  * エフェクトなどが混在している素材の加工
  * その他、Animatorなどで動かした際に違和感のある部分の調整対応
* パララックスな背景の実装
* 簡易的なミニマップの実装
* ScriptableObjectを用いてWave単位でスポーンを区切り、WaveをまとめたものをStageとして運用
  * https://github.com/skonishi1125/2d_arena_action_unity/blob/efc5c28d28a8aa4f5afe1cba469a672192768962/Assets/Scripts/Manager/WaveManager.cs
    * https://github.com/skonishi1125/2d_arena_action_unity/blob/efc5c28d28a8aa4f5afe1cba469a672192768962/Assets/Scripts/Etc/StageConfig.cs
      * https://github.com/skonishi1125/2d_arena_action_unity/blob/efc5c28d28a8aa4f5afe1cba469a672192768962/Assets/Scripts/Etc/WaveConfig.cs

#### その他
* バージョン管理※アニメーション等の外部素材はignore済
* DOTween等外部ライブラリの使用


### お借りした素材など
```
プレイヤー・敵・クリスタル・タイルセット
https://pixramen.itch.io/
https://creativekind.itch.io/floating-magic-crystal
https://tienlev.itch.io/slime-pixel-set

VFX
https://bdragon1727.itch.io/free-smoke-fx-pixel-2
https://parasaito.itch.io/
https://bdragon1727.itch.io/free-effect-and-bullet-16x16

スキルアイコン
https://kurai7.itch.io/

効果音
https://soundeffect-lab.info/
http://www.kurage-kosho.info/
https://umipla.com/
https://jdsherbert.itch.io/pixel-ui-sfx-pack

背景
https://edermunizz.itch.io/

BGM
https://www.youtube.com/@-misogi-misogi

フォント
3x4 dot font
https://piano-no-renshu.itch.io/3x4-dot-font

ベストテンFONT
https://flopdesign.booth.pm/items/2747965

```

