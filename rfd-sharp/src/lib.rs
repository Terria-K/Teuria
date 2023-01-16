use std::os::raw::c_char;
use std::ffi::CStr;

#[repr(C)]
struct PathHandler {
    pub error: u8,
    pub path: *const u8
}

impl PathHandler {
    const fn fail(error: u8) -> Self {
        PathHandler { error, path: std::ptr::null() }
    }

    const fn success(path: *const u8) -> Self {
        PathHandler { error: 64, path }
    }
}

#[no_mangle]
unsafe extern fn save_file(directory: *const c_char) -> PathHandler {
    let str_path = to_string_inner(directory)
        .unwrap_or("/");

    open_file_inner(str_path, Vec::new(), true)
}

#[no_mangle]
unsafe extern fn save_file_with_filter(
    directory: *const c_char, 
    filters: *const *const c_char,
    length: usize
) -> PathHandler {
    let str_path = to_string_inner(directory)
        .unwrap_or("/");

    let filter_data: &[*const c_char] = std::slice::from_raw_parts(filters, length);

    let filter_vec = filter_data_inner(filter_data, length);
    let filter_vec = match filter_vec {
        Ok(filter_vec) => filter_vec,
        Err(error) => return error
    };

    open_file_inner(str_path, filter_vec, true)
}

#[no_mangle]
unsafe extern fn open_file(directory: *const c_char) -> PathHandler {
    let str_path = to_string_inner(directory)
        .unwrap_or("/");

    open_file_inner(str_path, Vec::new(), false)
}

#[no_mangle]
unsafe extern fn open_file_with_filter(
    directory: *const c_char, 
    filters: *const *const c_char,
    length: usize
) -> PathHandler {
    let str_path = to_string_inner(directory)
        .unwrap_or("/");

    let filter_data: &[*const c_char] = std::slice::from_raw_parts(filters, length);

    let filter_vec = filter_data_inner(filter_data, length);
    let filter_vec = match filter_vec {
        Ok(filter_vec) => filter_vec,
        Err(error) => return error
    };

    open_file_inner(str_path, filter_vec, false)
}

fn to_string_inner<'a>(str_ptr: *const c_char) -> Option<&'a str> {
    let c_str = unsafe { CStr::from_ptr(str_ptr) };
    let c_str = c_str.to_str();
    
    let Ok(str_path) = c_str else { 
        return None;
    };
    Some(str_path)
}

fn filter_data_inner(filter_data: &[*const c_char], length: usize) -> Result<Vec<&str>, PathHandler> {
    let mut filter_vec = Vec::new();
    for i in filter_data.iter().take(length) {
        let c_str = unsafe {
            CStr::from_ptr(*i).to_str()
        };
        let Ok(str_filter) = c_str else { return Err(PathHandler::fail(0)) };
        filter_vec.push(
            str_filter
        );
    }
    Ok(filter_vec)
}

fn open_file_inner(directory: &str, filters: Vec<&str>, is_save: bool) -> PathHandler {
    let mut rfd = rfd::FileDialog::new()
        .set_directory(directory);
    
    if !filters.is_empty() {
        rfd = rfd.add_filter("Supported", &filters);
    }
    
    let files = if is_save {
        rfd.save_file()
    } else {
        rfd.pick_file()
    };

    
    if let Some(files) = files {
        let Some(files) = files.to_str() else { 
            return PathHandler::fail(0)
        };
        PathHandler::success(files.as_ptr())
    } else {
        PathHandler::fail(1)
    }
}
