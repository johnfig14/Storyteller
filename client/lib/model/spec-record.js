var Immutable = require('immutable');
var Specification = require('./specification');
var Counts = require('./counts');
var _ = require('lodash');

function statusForResults(results){
    if (!results) return 'none';
    
    var counts = new Counts(results.results.counts);
    if (counts.success()) return 'success';
    
    return 'failed';
}

class SpecRecord extends Immutable.Record({id: null, spec: null, version: 0, last_result: null, status: 'none'}) {
    constructor(data, library, last_result){
        var spec = new Specification(data, library);

        super({id: data.id, spec: spec, version: 0, last_result: last_result, status: statusForResults(last_result)});
    }
    
    get lifecycle() {
        return this.spec.lifecycle;
    }
    
    get title(){
        return this.spec.title;
    }
    
    get mode(){
        return this.spec.mode;
    }
    
    get activeContainer(){
        return this.spec.activeContainer;
    }

    isDirty(){
        return this.spec.isDirty();
    }
    
    canRedo(){
        return this.spec.canRedo();
    }
    
    canUndo(){
        return this.spec.canUndo();
    }
    
    editors(loader){
        return this.spec.editors(loader);
    }
    
    outline(){
        return this.spec.outline();
    }
    
    buildResults(loader){
        return this.spec.buildResults(loader);
    }
    
    previews(loader){
        return this.spec.previews(loader);
    }
    
    replace(spec){
        return this.set('spec', spec);
    }
    
    recordLastResult(result){
        return this.set('last_result', result).set('status', statusForResults(result));
    }
    
    clearResults(){
        return this.set('last_result', null).set('status', 'none');
    }
    
    hasResults(){
        return this.last_result != null;
    }
    
    icon(running, queued, progress){
    
        if (this.id == running){
            if (progress.counts.anyResults()){
                if (progress.counts.success()){
                    return 'running-success';
                }
                else {
                    return 'running-failed';
                }
            }
            else {
                return 'running';
            }
        }
        else if (_.includes(queued, this.id)){
            return 'queued';
        }
        else if (this.hasResults()){
            return this.status;
        }
        else {
            return 'none';
        }


    }

    acceptChange(func){
        var version = this.get('version') + 1;
        func(this.spec);
        
        return this.set('version', version)
    }

}

module.exports = SpecRecord;