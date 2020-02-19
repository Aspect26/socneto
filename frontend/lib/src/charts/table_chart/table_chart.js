class TableChart {

    create(selector, dataSet) {
        this._removeOld(selector);
    }

    _removeOld(selector) {
        d3.select(selector).selectAll("material-progress").remove();
        //d3.select(selector).html("");
    }

}