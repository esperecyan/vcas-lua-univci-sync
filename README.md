esperecyan/vcas-lua-univci-sync
===============================
EmbeddedScriptWorkspaceフォルダ内の .lua ファイルが更新されたときに、Unityプロジェクト内のVCIプレハブのLuaデータを更新する、Unityエディタ拡張です。

UniVCI-0.35.0で、Luaスクリプトの参照先としてファイルパスを指定する機能が復活しましたが、バージョン管理しづらいため、本スクリプトを作成しました。

- EmbeddedScriptWorkspaceフォルダ直下のフォルダ名と、拡張子を除くファイル名が同一のプレハブに対して適用されます。
- VCIObjectコンポーネントはプレハブのルートオブジェクトにアタッチされている必要があります。
- VCIObjectのスクリプトにファイルパスが指定されていて、それが更新されたファイルと同一の場合は、何もしません。

プロジェクトへのインストール
----------------------------
```sh
npm install -g openupm-cli
openupm add jp.pokemori.vcas-lua-univci-sync
```

参照: https://openupm.com/packages/jp.pokemori.vcas-lua-univci-sync
