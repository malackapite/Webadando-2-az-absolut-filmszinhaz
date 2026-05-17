angular.module('catApp')
.factory('CatModel', function($http) {
    const API_BASE_URL = "https://localhost:7063";
    let cart = {}; // Struktúra: { macskaId: darabszam }

    return {
        // READ
        loadCats: function() {
            return $http.get(`${API_BASE_URL}/macska`).then(resp => resp.data);
        },

        findCatById: function(id) {
            return this.loadCats().then(cats => cats.find(cat => cat.id == id));
        },

        // CREATE
        saveCat: function(newCat) {
            return $http.post(`${API_BASE_URL}/macska`, newCat).then(resp => resp.data
            ).catch(err => {
                console.error("Hiba a mentés során:", err);
            });
        },

        // UPDATE
        updateCat: function(cat) {
            return $http.patch(`${API_BASE_URL}/macska`, cat).then(
                resp => resp.data
            ).catch(err => {
                console.error("Hiba a módosítás során:", err);
            });
        },

        // DELETE
        deleteCat: function(id) {
            return $http.delete(`${API_BASE_URL}/macska/${id}`).then(resp => {
                return resp.data;
            }).catch(err => {
                console.error("Hiba a törlés során:", err);
            });
        },

        getCart: () => cart,

        addToCart: (id, qty) => { cart[id] = (cart[id] || 0) + parseInt(qty); },

        deleteCart: () => { cart = {}; },
        
        removeItem: function(id) {
            delete cart[id];
        },

    };
});