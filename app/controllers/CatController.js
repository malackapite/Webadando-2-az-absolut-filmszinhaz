angular.module('catApp')
.controller('CatController', function($scope, CatModel, $location, $timeout) {
    $scope.cats = [];
    $scope.nth = 0;
    $scope.vasaroltDb = 1;
    let player = null;

    CatModel.loadCats().then(data => {
        $scope.cats = data;
        init3D();
    });

    function init3D() {
        $timeout(function() {
            const container = document.getElementById("macska3D");
            
            if (!container || !window.APP || !window.THREE) {
                console.error("Three.js vagy a konténer nem található!");
                return;
            }

            const loader = new window.THREE.FileLoader();
            loader.load('app.json', function (text) {
                // Új player példány
                player = new window.APP.Player();
                player.load(JSON.parse(text));
                
                // Méretezés a div szélességéhez
                player.setSize(container.clientWidth, 400);
                player.play();
                if (player && typeof player.setupCats === 'function') {
                    player.setupCats($scope.cats.length);
                }
                
                container.innerHTML = ""; // Ürítjük a konténert az újratöltés megelőzésére
                container.appendChild(player.dom);
                
                // Kezdeti állapot átadása a Three.js-nek
                if (typeof player.setNth === 'function') {
                    player.setNth($scope.nth);
                }
            });
        }, 100);
    }

    // Lapozás
    $scope.lapoz = function(irany) {
        let ujIndex = $scope.nth + irany;
        if (ujIndex >= 0 && ujIndex < $scope.cats.length) {
            $scope.nth = ujIndex;
            $scope.vasaroltDb = 1;
            if (player) player.setNth($scope.nth); // 3D modell frissítése
        }
    };

    $scope.ugrik = function(index) {
        $scope.nth = index;
        $scope.vasaroltDb = 1;
        if (player) player.setNth($scope.nth); // 3D modell frissítése
    };

    // Kosár kezelés
    $scope.kosarlista = CatModel.getCart();

    $scope.vasarol = function() {
        let currentCat = $scope.cats[$scope.nth];
        if ($scope.vasaroltDb > 0 && $scope.vasaroltDb < 100) {
            CatModel.addToCart(currentCat.id, $scope.vasaroltDb);
            new bootstrap.Toast($('#sikeresVasarlas')).show();
        } else {
            new bootstrap.Toast($('#liveToast')).show();
        }
    };

    $scope.getCatById = function(id) {
        return $scope.cats.find(c => c.id == id);
    };

    $scope.getKosarDb = function() {
        return Object.keys($scope.kosarlista).length;
    };

    $scope.torol = function(id) {
        CatModel.removeItem(id);
    };

    $scope.fizetes = function() {
        $('#kosarPanel').modal('hide');
        $location.path('/checkout');
    };
});