"use strict";
angular
    .module("app", [
        "ngAnimate",
        "ngRoute",
        "loading-spinner",
        "ngFileUpload",
        "ui.bootstrap"
    ])
    .config(function ($routeProvider, $locationProvider) {

        $routeProvider
            .when("/", {
                templateUrl: "app/home/home.html",
                controller: "HomeCtrl",
                controllerAs: "home"
            })
            .otherwise({
                redirectTo: "/"
            });

        $locationProvider.html5Mode(true);

    })
    .controller('ModalCtrl', ['$rootScope', '$uibModal', '$log', '$document', 'fileService',
        function ($rootScope, $uibModal, $log, $document, fileService) {
        var $ctrl = this;
        $ctrl.items = [];
        $ctrl.animationsEnabled = true;
        $rootScope.images = [];
        $ctrl.open = function (size, name, parentSelector) {

            $ctrl.items = [];
            $rootScope.images = [];
            $rootScope.images.push(name);

            console.log($rootScope.images);

            fileService.processImage(name)
                .then(function (data) {
                    
                    data.data.map(function (i) {
                        $ctrl.items.push(i);
                    });
                });
            
            var parentElem = parentSelector ?
                angular.element($document[0].querySelector('.modal-demo ' + parentSelector)) : undefined;
            var modalInstance = $uibModal.open({
                animation: $ctrl.animationsEnabled,
                ariaLabelledBy: 'modal-title',
                ariaDescribedBy: 'modal-body',
                templateUrl: 'myModalContent.html',
                controller: 'ModalInstanceCtrl',
                controllerAs: '$ctrl',
                size: size,
                appendTo: parentElem,
                resolve: {
                    items: function () {
                        return $ctrl.items;
                    }
                }
            });

            console.log($rootScope.images);

            modalInstance.result.then(function (selectedItem) {
                $ctrl.selected = selectedItem;
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };

        $ctrl.openComponentModal = function () {
            var modalInstance = $uibModal.open({
                animation: $ctrl.animationsEnabled,
                component: 'modalComponent',
                resolve: {
                    items: function () {
                        return $ctrl.items;
                    }
                }
            });

            modalInstance.result.then(function (selectedItem) {
                $ctrl.selected = selectedItem;
            }, function () {
                $log.info('modal-component dismissed at: ' + new Date());
            });
        };

        $ctrl.openMultipleModals = function () {
            $uibModal.open({
                animation: $ctrl.animationsEnabled,
                ariaLabelledBy: 'modal-title-bottom',
                ariaDescribedBy: 'modal-body-bottom',
                templateUrl: 'stackedModal.html',
                size: 'sm',
                controller: function ($scope) {
                    $scope.name = 'bottom';
                }
            });

            $uibModal.open({
                animation: $ctrl.animationsEnabled,
                ariaLabelledBy: 'modal-title-top',
                ariaDescribedBy: 'modal-body-top',
                templateUrl: 'stackedModal.html',
                size: 'sm',
                controller: function ($scope) {
                    $scope.name = 'top';
                }
            });
        };

        $ctrl.toggleAnimation = function () {
            $ctrl.animationsEnabled = !$ctrl.animationsEnabled;
        };
    }])
    .controller('ModalInstanceCtrl', function ($uibModalInstance, items) {
        var $ctrl = this;
        $ctrl.items = items;
        $ctrl.selected = {
            item: $ctrl.items[0]
        };

        $ctrl.ok = function () {
            $uibModalInstance.close($ctrl.selected.item);
        };

        $ctrl.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    })
    .component('modalComponent', {
        templateUrl: 'myModalContent.html',
        bindings: {
            resolve: '<',
            close: '&',
            dismiss: '&'
        },
        controller: function () {
            var $ctrl = this;

            $ctrl.$onInit = function () {
                $ctrl.items = $ctrl.resolve.items;
                $ctrl.selected = {
                    item: $ctrl.items[0]
                };
            };

            $ctrl.ok = function () {
                $ctrl.close({ $value: $ctrl.selected.item });
            };

            $ctrl.cancel = function () {
                $ctrl.dismiss({ $value: 'cancel' });
            };
        }
    })
    .constant("apiUrl", "api/image/");
    