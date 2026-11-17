import { createStore } from 'vuex'
import system from './modules/system'
import user from './modules/user'
import dict from './modules/dict'
import tagsView from './modules/tagsView'

export default createStore({
  modules: {
    system,
    user,
    dict,
    tagsView
  }
})
