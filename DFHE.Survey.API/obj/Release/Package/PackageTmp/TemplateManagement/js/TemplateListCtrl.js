var SurveyApp = angular.module('dfhe.app');
(function (ng, app) {
    function TemplateListCtrl($scope, $rootScope, apiService, loginInfo, AuthenticationService, alert) {
        this.scope = $scope;
        this.rootScope = $rootScope;
        this.loginInfo = loginInfo;
        this.authService = AuthenticationService;
        this.apiService = apiService;
        this.alert = alert;
        this._init();
    }
    TemplateListCtrl.prototype = {
        _init: function () {
            this._setScope();
        },
        _setScope: function () {
            var _this = this;
            _this.rootScope.EmployeeInfo = _this.loginInfo;
            _this.rootScope.logout = function () { _this.authService.logout(); };
            _this.scope.ret = {};
            _this.scope.QrCodeBase64Str = 'loading...';
            _this.scope.template = {};
            _this._registerMethod();
            _this.scope.getTemplateList();
        },
        _registerMethod: function () {
            var _this = this;
           
            _this.scope.getTemplateList = function () {
                _this.apiService.get({
                    controller: 'Template',
                    action: 'GetTemplateList'
                }, function (res) {
                    if (res.Result == 1) {
                        //angular.forEach(res.Data, function (v, k) {
                        //    v.StaticUrl = "http://" + location.host + '/' + v.StaticUrl;
                        //});
                        _this.scope.ret = res.Data;
                    }
                }, function (res) {

                });
            },

            _this.scope.downloadTmpl = function(id) {
                _this.apiService.get({
                    controller: 'Template',
                    action: 'DownloadTemplate',
                    id: id
                }, function(res) {

                }, function(res) {

                });
            },
            _this.scope.previewTmpl = function (storedName) {

                var urlArr = storedName.split('\\');
                var tmpArr = [urlArr[urlArr.length - 3], urlArr[urlArr.length - 2], urlArr[urlArr.length - 1]];
                var target = tmpArr.join('\\');
                target = target + '\\example.html';
               

                var url = target;  //转向网页的地址; 
                var name = 'Example';                          //网页名称，可为空; 
                var iWidth = 640;                          //弹出窗口的宽度; 
                var iHeight = 900;                         //弹出窗口的高度; 
                //获得窗口的垂直位置 
                var iTop = (window.screen.availHeight - 30 - iHeight) / 2;
                //获得窗口的水平位置 
                var iLeft = (window.screen.availWidth - 10 - iWidth) / 2;
                window.open(url, name, 'height=' + iHeight + ',,innerHeight=' + iHeight + ',width=' + iWidth + ',innerWidth=' + iWidth + ',top=' + iTop + ',left=' + iLeft + ',status=no,toolbar=no,menubar=no,location=no,resizable=no,scrollbars=0,titlebar=no');
                //window.open('/statics/20150506172401.html', 'newwindow', 'height=900,width=640,top=0,left=0,toolbar=no,menubar=no,scrollbars=no, resizable=no,location=no, status=no');
            },
            //_this.scope.deleteTemplateById = function(id) {
            //    _this.apiService.delete({
            //        controller: 'Template',
            //        action: 'DeleteTemplateById',
            //        id: id
            //    }, function(res) {
            //        if (res.Result == 1) {
            //            _this.alert.success('模版删除成功');
            //        } else {                        
            //            _this.alert.error('模版删除失败');
            //        }
            //    }, function(res) {
            //        _this.alert.error('模版删除失败');
            //    });
            //    scope.$emit('ui-refresh', true);
            //},
             _this.scope.$on('ui-refresh', function (event, data) {
                 if (data) {
                     _this.scope.getTemplateList();
                 }
             });
        }
    }
    app.controller('TemplateListCtrl', TemplateListCtrl)
        .directive("uploadTmpl", function($rootScope, $modal, apiService, $upload, alert) {
            return {
                restrict: "A",
                scope: {
                    pro: "="
                },
                link: function(scope, element, attr) {


                    element.click(function() {

                        scope.template = {};
                        scope.isEmpty = false;
                        scope.extError = false;
                        scope.fileArr = [];
                        // 显示数据
                        scope.modal = $modal({
                            scope: scope,
                            animation: "am-fade-and-scale",
                            template: "/Templates/_uploadTmplModal.html"
                        });
                    });

                    scope.submitData = function() {

                            //$apiService.post({
                            //    controller: "Testing",
                            //    action: "TestingFeed"
                            //}, JSON.stringify(scope.feed), function (res) {
                            //    if (res) {
                            //        if (res.Result == 1) {
                            //            $sysAlter("保存成功", 'success');
                            //            scope.modal.hide();
                            //            _this.scope.$emit('to-parent', true);
                            //        }
                            //        else
                            //            $sysAlter(res.ErrorMsg, 'danger');
                            //    }
                            //}, function (err) {
                            //    $sysAlter(JSON.stringify(err), 'danger');
                            //});
                        },
                        scope.insertTmpl = function() {

                            if (scope.fileArr.length < 1) {
                                scope.isEmpty = true;
                                return;
                            }

                            if (scope.extError) {
                                alert.error('操作失败：只能上传.zip文件!');
                                return;
                            }

                            var template = {
                                TmplTitle: scope.template.TmplTitle,
                                TmplDescription: scope.template.TmplDescription,
                                PreviewUrl: '',
                                StoredName: scope.tempPath,
                                UserName: $rootScope.EmployeeInfo.UserName
                            };

                            apiService.post({
                                controller: 'Template',
                                action: 'InsertTemplate'
                            }, angular.toJson(template), function(res) {
                                scope.modal.hide();
                                alert.success('新增模版成功');
                                scope.$emit('ui-refresh', true);
                            }, function(res) {
                                alert.error('新增模版失败');
                            });
                        },
                        scope.onFileSelect = function($files) {
                            var arrLength = $files.length;
                            for (var i = 0; i < arrLength; i++) {

                                var tmp = $files[i];
                                var ext = tmp.name.split('.')[1];
                                if (ext != 'zip' && ext != 'ZIP') {
                                    scope.extError = true;
                                    alert.error('操作失败：只能上传.zip文件!');
                                    //$files[i].name = '';
                                    return;
                                }

                                scope.upload = $upload.upload({
                                    url: 'api/Template/UploadTemplate',
                                    //data: {},
                                    file: $files[i]
                                }).progress(function(evt) {
                                }).success(function(data, status, headers, config) { // file is uploaded successfully
                                    scope.tempPath = data.Data.tempPath;
                                    scope.fileArr.push(data);
                                    if (scope.fileArr.length > 0) {
                                        scope.isEmpty = false;
                                    }
                                });
                            };
                        };
                }
            }
        })
        .filter('tempExampleUrlFilter', function() {
            return function(n) {
                var urlArr = n.split('\\');
                var tmpArr = [urlArr[urlArr.length - 3], urlArr[urlArr.length - 2], urlArr[urlArr.length - 1]];
                var target = tmpArr.join('\\');
                target = target + '\example.html';
                return target;
            }
        });

})(angular, SurveyApp);