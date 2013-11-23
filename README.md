pomelo-u3d-client-helper
========================

解决pomelo u3d client request函数不能调用只能在主线程里调用的函数
直接将文件拷贝到项目文件夹。
使用方法：

ClientManager clientManager = ClientManager.Instance();

clientManager.Request("user.userHandler.login", null, (data)=>{
	int code = Convert.ToInt32(data["code"]);
	if (code == (int)Code.OK) {
		Application.LoadLevel("MainLevel");
	}
	else {
		showErrorDialog("Error", "Invalid user or password");
	}
});

ClientManager很粗糙， 希望能给为此苦闷的同学们一些思路